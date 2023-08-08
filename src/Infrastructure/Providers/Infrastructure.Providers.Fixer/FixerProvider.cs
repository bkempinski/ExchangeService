using Core.Contract.Providers.Requests.ExchangeRateProvider;
using Core.Contract.Providers.Responses.ExchangeRateProvider;
using Core.Domain.Abstraction;
using Core.Domain.Exceptions;
using Infrastructure.Providers.Fixer.Models;
using Infrastructure.Providers.Fixer.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Shared.Infrastructure.Exceptions;
using System.Net.Http.Json;

namespace Infrastructure.Providers.Fixer;

public class FixerProvider : IExchangeRateProvider
{
    private readonly ILogger<FixerProvider> _logger;
    private readonly IOptions<FixerOptions> _options;
    private readonly HttpClient _httpClient;

    public FixerProvider
        (
            ILogger<FixerProvider> logger,
            IOptions<FixerOptions> options,
            HttpClient httpClient
        ) => (_logger, _options, _httpClient) 
            = (logger, options, httpClient);

    public string Name => "fixer.io";

    public async Task<GetExchangeRateResponse> GetExchangeRateAsync(GetExchangeRateRequest request)
    {
        _logger.LogDebug($"FixerProvider -> GetExchangeRateAsync -> Request: {request}");

        if (request == null)
            throw new Core.Domain.Exceptions.ArgumentNullException(nameof(request));

        if (string.IsNullOrEmpty(request.CurrencyTo))
            throw new Core.Domain.Exceptions.ArgumentNullException(nameof(request.CurrencyTo));

        if (string.IsNullOrEmpty(request.CurrencyFrom))
            throw new Core.Domain.Exceptions.ArgumentNullException(nameof(request.CurrencyFrom));

        if (string.IsNullOrEmpty(_options.Value?.ApiAccessKey))
            throw new Core.Domain.Exceptions.ArgumentNullException("ApiAccessKey");

        var url = $"latest?access_key={_options.Value?.ApiAccessKey}&base={request.CurrencyFrom}&symbols={request.CurrencyTo}&format=1";
        var response = await _httpClient.GetFromJsonAsync<LatestResponse>(url);

        _logger.LogDebug($"FixerProvider -> GetExchangeRateAsync -> Response: {response}");

        if (response.Success)
            return new GetExchangeRateResponse
            {
                CurrencyTo = request.CurrencyTo,
                CurrencyFrom = request.CurrencyFrom,
                ExchangeRate = response.Rates?
                    .Where(r => r.Key.Equals(request.CurrencyTo, StringComparison.InvariantCultureIgnoreCase))
                    .Select(r => r.Value)
                    .DefaultIfEmpty(1m)
                    .FirstOrDefault() ?? 1m,
                Date = response.Date.ToDateTime(TimeOnly.MinValue)
            };
        else
            throw new ProviderException(response.Error?.Info);
    }
}
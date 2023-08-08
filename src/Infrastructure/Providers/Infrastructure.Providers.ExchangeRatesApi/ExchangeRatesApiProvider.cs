using Core.Contract.Providers.Requests.ExchangeRateProvider;
using Core.Contract.Providers.Responses.ExchangeRateProvider;
using Core.Domain.Abstraction;
using Infrastructure.Providers.ExchangeRatesApi.Models;
using Infrastructure.Providers.ExchangeRatesApi.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Shared.Infrastructure.Exceptions;
using System.Net.Http.Json;

namespace Infrastructure.Providers.ExchangeRatesApi;

public class ExchangeRatesApiProvider : IExchangeRateProvider
{
    private readonly ILogger<ExchangeRatesApiProvider> _logger;
    private readonly IOptions<ExchangeRatesApiOptions> _options;
    private readonly HttpClient _httpClient;

    public ExchangeRatesApiProvider
        (
            ILogger<ExchangeRatesApiProvider> logger,
            IOptions<ExchangeRatesApiOptions> options,
            HttpClient httpClient
        ) => (_logger, _options, _httpClient)
            = (logger, options, httpClient);

    public string Name => "exchangeratesapi.io";

    public async Task<GetExchangeRateResponse> GetExchangeRateAsync(GetExchangeRateRequest request)
    {
        _logger.LogDebug($"ExchangeRatesApiProvider -> GetExchangeRateAsync -> Request: {request}");

        if (request == null)
            throw new Core.Domain.Exceptions.ArgumentNullException(nameof(request));

        if (string.IsNullOrEmpty(request.CurrencyFrom))
            throw new Core.Domain.Exceptions.ArgumentNullException(nameof(request.CurrencyFrom));

        if (string.IsNullOrEmpty(request.CurrencyTo))
            throw new Core.Domain.Exceptions.ArgumentNullException(nameof(request.CurrencyTo));

        if (string.IsNullOrEmpty(_options.Value?.ApiAccessKey))
            throw new Core.Domain.Exceptions.ArgumentNullException("ApiAccessKey");

        var url = $"latest?access_key={_options.Value?.ApiAccessKey}&base={request.CurrencyFrom}&symbols={request.CurrencyTo}&format=1";
        var response = await _httpClient.GetFromJsonAsync<LatestResponse>(url);

        _logger.LogDebug($"ExchangeRatesApiProvider -> GetExchangeRateAsync -> Response: {response}");

        if (response.Success)
            return new GetExchangeRateResponse
            {
                CurrencyFrom = request.CurrencyFrom,
                CurrencyTo = request.CurrencyTo,
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
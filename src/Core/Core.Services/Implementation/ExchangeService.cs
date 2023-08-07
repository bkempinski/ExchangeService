using Core.Contract.Providers.Requests.ExchangeRateProvider;
using Core.Contract.Providers.Responses.ExchangeRateProvider;
using Core.Contract.Services.Requests.CacheService;
using Core.Contract.Services.Requests.ExchangeService;
using Core.Contract.Services.Responses.ExchangeService;
using Core.Domain.Abstraction;
using Core.Domain.Exceptions;
using Core.Services.Abstraction;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Core.Services.Implementation;

public class ExchangeService : IExchangeService
{
    private readonly ILogger<ExchangeService> _logger;
    private readonly IConfiguration _configuration;
    private readonly ICacheService _cacheService;
    private readonly IEnumerable<IExchangeRateProvider> _exchangeRateProviders;

    public ExchangeService
        (
            ILogger<ExchangeService> logger,
            IConfiguration configuration,
            ICacheService cacheService,
            IEnumerable<IExchangeRateProvider> exchangeRateProviders
        ) => (_logger, _configuration, _cacheService, _exchangeRateProviders) 
            = (logger, configuration, cacheService, exchangeRateProviders);

    public async Task<ConvertCurrencyResponse> ConvertCurrencyAsync(ConvertCurrencyRequest request)
    {
        _logger.LogDebug($"ExchangeService -> ConvertCurrencyAsync -> Request: {request}");

        if (request == null)
            throw new Domain.Exceptions.ArgumentNullException(nameof(request));

        var exchangeRateProviderName = request.ExchangeRateProviderName ?? _configuration.GetValue<string>("DefaultExchangeRateProviderName");

        _logger.LogDebug($"ExchangeService -> ConvertCurrencyAsync -> ExchangeRateProviderName: {exchangeRateProviderName}");

        if (string.IsNullOrEmpty(exchangeRateProviderName))
            throw new Domain.Exceptions.ArgumentNullException(nameof(exchangeRateProviderName));

        var exchangeRateProvider = _exchangeRateProviders?.FirstOrDefault(erp => erp.Name.Equals(exchangeRateProviderName, StringComparison.InvariantCultureIgnoreCase));

        if (exchangeRateProvider == null)
            throw new NotFoundException($"ExchangeRateProvider not found - {exchangeRateProviderName}");

        var exchangeRate = await _cacheService.GetOrAddDistributedAsync(new GetOrAddRequest<GetExchangeRateResponse>
        {
            CacheKey = CacheKeys.GetExchangeRate(exchangeRateProviderName, request.FromCurrency, request.Currency),
            AbsoluteExpiration = _configuration.GetValue("ExchangeRateExpiration", TimeSpan.FromMinutes(30)),
            ValueFactory = () => exchangeRateProvider.GetExchangeRateAsync(new GetExchangeRateRequest
            {
                Currency = request.Currency,
                BaseCurrency = request.FromCurrency
            })
        });

        return new ConvertCurrencyResponse
        {
            Currency = request.Currency,
            Value = request.FromValue * exchangeRate.Value.ExchangeRate,
            FromCurrency = request.FromCurrency,
            FromValue = request.FromValue
        };
    }
}
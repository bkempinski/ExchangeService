using Core.Contract.Providers.Requests.ExchangeRateProvider;
using Core.Contract.Providers.Responses.ExchangeRateProvider;
using Core.Contract.Services.Requests.CacheService;
using Core.Contract.Services.Requests.ExchangeService;
using Core.Contract.Services.Responses.ExchangeService;
using Core.Domain.Abstraction;
using Core.Domain.Abstraction.Repositories;
using Core.Domain.Entities;
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
    private readonly IClientRepository _clientRepository;
    private readonly ITradeRepository _tradeRepository;

    public ExchangeService
        (
            ILogger<ExchangeService> logger,
            IConfiguration configuration,
            ICacheService cacheService,
            IEnumerable<IExchangeRateProvider> exchangeRateProviders,
            IClientRepository clientRepository,
            ITradeRepository tradeRepository
        ) => (_logger, _configuration, _cacheService, _exchangeRateProviders, _clientRepository, _tradeRepository)
            = (logger, configuration, cacheService, exchangeRateProviders, clientRepository, tradeRepository);

    public async Task<CurrencyConvertResponse> CurrencyConvertAsync(CurrencyConvertRequest request)
    {
        // Validation
        _logger.LogDebug($"ExchangeService -> CurrencyConvertAsync -> Request: {request}");

        if (request == null)
            throw new Domain.Exceptions.ArgumentNullException(nameof(request));

        if (string.IsNullOrEmpty(request.CurrencyFrom))
            throw new Domain.Exceptions.ArgumentNullException(nameof(request.CurrencyFrom));

        if (string.IsNullOrEmpty(request.CurrencyTo))
            throw new Domain.Exceptions.ArgumentNullException(nameof(request.CurrencyTo));

        var exchangeRateProviderName = request.ExchangeRateProviderName ?? _configuration.GetValue<string>("DefaultExchangeRateProviderName");

        _logger.LogDebug($"ExchangeService -> CurrencyConvertAsync -> ExchangeRateProviderName: {exchangeRateProviderName}");

        if (string.IsNullOrEmpty(exchangeRateProviderName))
            throw new Domain.Exceptions.ArgumentNullException(nameof(exchangeRateProviderName));

        var exchangeRateProvider = _exchangeRateProviders?.FirstOrDefault(erp => erp.Name.Equals(exchangeRateProviderName, StringComparison.InvariantCultureIgnoreCase));

        if (exchangeRateProvider == null)
            throw new NotFoundException($"ExchangeRateProvider not found - {exchangeRateProviderName}");

        // Currency convert (cached)
        var exchangeRate = await _cacheService.GetOrAddDistributedAsync(new GetOrAddRequest<GetExchangeRateResponse>
        {
            CacheKey = CacheKeys.GetExchangeRate(exchangeRateProviderName, request.CurrencyFrom, request.CurrencyTo),
            AbsoluteExpiration = _configuration.GetValue("ExchangeRateExpiration", TimeSpan.FromMinutes(30)),
            ValueFactory = () => exchangeRateProvider.GetExchangeRateAsync(new GetExchangeRateRequest
            {
                CurrencyTo = request.CurrencyTo,
                CurrencyFrom = request.CurrencyFrom
            })
        });

        return new CurrencyConvertResponse
        {
            CurrencyFrom = request.CurrencyFrom,
            CurrencyTo = request.CurrencyTo,
            ExchangeRate = exchangeRate.Value.ExchangeRate,
            Value = request.Value * exchangeRate.Value.ExchangeRate
        };
    }

    public async Task<CurrencyTradeResponse> CurrencyTradeAsync(CurrencyTradeRequest request)
    {
        // Validation
        _logger.LogDebug($"ExchangeService -> CurrencyTradeAsync -> Request: {request}");

        if (request == null)
            throw new Domain.Exceptions.ArgumentNullException(nameof(request));

        if (string.IsNullOrEmpty(request.ClientIpAddress))
            throw new Domain.Exceptions.ArgumentNullException(nameof(request.ClientIpAddress));

        var currencyConvert = await CurrencyConvertAsync(new CurrencyConvertRequest
        {
            CurrencyFrom = request.CurrencyFrom,
            CurrencyTo = request.CurrencyTo,
            Value = request.Value,
            ExchangeRateProviderName = null
        });

        // Trade
        var client = await GetOrCreateClientByIpAddressAsync(request.ClientIpAddress);

        // Check number of trades (cached)
        var tradesCount = await _cacheService.GetOrAddDistributedAsync(new GetOrAddRequest<int>
        {
            CacheKey = CacheKeys.GetTradesCount(client.Id),
            AbsoluteExpiration = _configuration.GetValue("TradesCountExpiration", TimeSpan.FromHours(1)),
            ValueFactory = () => _tradeRepository.CountTradesAsync(client.Id, DateTime.UtcNow.AddHours(-1))
        });

        _logger.LogDebug($"ExchangeService -> CurrencyTradeAsync -> Trades count: {tradesCount}, Client ID: {client.Id}");

        if (tradesCount.Value < _configuration.GetValue("TradesCountLimit", 10))
        {
            // Insert new trade
            var trade = await _tradeRepository.UpsertAsync(new Trade
            {
                Id = 0,
                ClientId = client.Id,
                CurrencyFrom = request.CurrencyFrom,
                CurrencyTo = request.CurrencyTo,
                ValueFrom = request.Value,
                ValueTo = currencyConvert.Value,
                ExchangeRate = currencyConvert.ExchangeRate,
                Timestamp = DateTime.UtcNow
            });

            // Increase trades counter
            var newTradesCount = await _cacheService.SetValueDistributedAsync(new SetValueRequest<int>
            {
                CacheKey = CacheKeys.GetTradesCount(client.Id),
                AbsoluteExpiration = _configuration.GetValue("TradesCountExpiration", TimeSpan.FromHours(1)),
                Value = tradesCount.Value + 1
            });

            return new CurrencyTradeResponse
            {
                Success = true,
                Message = $"Trades count: {newTradesCount.Value}"
            };
        }
        else
            _logger.LogDebug($"ExchangeService -> CurrencyTradeAsync -> Trades limit reached: {tradesCount}, Client ID: {client.Id}");

        return new CurrencyTradeResponse
        {
            Success = false,
            Message = $"Trades limit reached - {tradesCount.Value}"
        };
    }

    private async Task<Client> GetOrCreateClientByIpAddressAsync(string ipAddress)
    {
        var client = await _clientRepository.GetByIpAddressAsync(ipAddress);

        if (client == null)
        {
            _logger.LogDebug($"ExchangeService -> GetOrCreateClientByIpAddressAsync -> New client: {ipAddress}");

            client = await _clientRepository.UpsertAsync(new Client
            {
                Id = 0,
                IpAddress = ipAddress
            });
        }
        else
            _logger.LogDebug($"ExchangeService -> GetOrCreateClientByIpAddressAsync -> Found client ID: {client.Id}");

        return client;
    }
}
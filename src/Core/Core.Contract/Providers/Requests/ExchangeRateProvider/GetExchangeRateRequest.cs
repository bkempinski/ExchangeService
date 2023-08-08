namespace Core.Contract.Providers.Requests.ExchangeRateProvider;

public record GetExchangeRateRequest
{
    public string CurrencyFrom { get; init; } = "EUR";
    public string CurrencyTo { get; init; }
}
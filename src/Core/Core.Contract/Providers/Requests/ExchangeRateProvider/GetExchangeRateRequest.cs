namespace Core.Contract.Providers.Requests.ExchangeRateProvider;

public record GetExchangeRateRequest
{
    public string Currency { get; init; }
    public string BaseCurrency { get; init; } = "EUR";
}
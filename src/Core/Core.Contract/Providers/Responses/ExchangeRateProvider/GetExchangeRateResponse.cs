namespace Core.Contract.Providers.Responses.ExchangeRateProvider;

public record GetExchangeRateResponse
{
    public string Currency { get; init; }
    public string BaseCurrency { get; init; }
    public decimal ExchangeRate { get; init; }
    public DateTime Date { get; init; }
}
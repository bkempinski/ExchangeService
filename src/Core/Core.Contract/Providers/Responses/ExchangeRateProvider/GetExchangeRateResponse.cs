namespace Core.Contract.Providers.Responses.ExchangeRateProvider;

public record GetExchangeRateResponse
{
    public string CurrencyFrom { get; init; }
    public string CurrencyTo { get; init; }
    public decimal ExchangeRate { get; init; }
    public DateTime Date { get; init; }
}
namespace Core.Contract.Services.Requests.ExchangeService;

public record CurrencyConvertRequest
{
    public string CurrencyFrom { get; init; } = "EUR";
    public string CurrencyTo { get; init; }
    public decimal Value { get; init; }
    public string ExchangeRateProviderName { get; set; } = null;
}
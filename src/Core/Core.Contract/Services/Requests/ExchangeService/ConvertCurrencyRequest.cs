namespace Core.Contract.Services.Requests.ExchangeService;

public record ConvertCurrencyRequest
{
    public string Currency { get; init; }
    public string FromCurrency { get; init; } = "EUR";
    public decimal FromValue { get; init; }
    public string ExchangeRateProviderName { get; set; } = null;
}
namespace Core.Contract.Services.Responses.ExchangeService;

public record ConvertCurrencyResponse
{
    public string Currency { get; init; }
    public decimal Value { get; init; }
    public string FromCurrency { get; init; }
    public decimal FromValue { get; init; }
}
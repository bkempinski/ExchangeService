namespace Core.Contract.Services.Responses.ExchangeService;

public record CurrencyConvertResponse
{
    public string CurrencyFrom { get; init; }
    public string CurrencyTo { get; init; }
    public decimal ExchangeRate { get; init; }
    public decimal Value { get; init; }
}
namespace Core.Contract.Services.Requests.ExchangeService;

public record CurrencyTradeRequest
{
    public string ClientIpAddress { get; init; }
    public string CurrencyFrom { get; init; } = "EUR";
    public string CurrencyTo { get; init; }
    public decimal Value { get; init; }
}
namespace Core.Contract.Services.Responses.ExchangeService;

public record CurrencyTradeResponse
{
    public bool Success { get; init; }
    public string Message { get; init; }
}
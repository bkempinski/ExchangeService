namespace Core.Domain.Models;

public record ExchangeRate
{
    public string BaseCurrency { get; init; }
    public DateTime Updated { get; init; }
    public Dictionary<string, decimal> Rates { get; init; }
}
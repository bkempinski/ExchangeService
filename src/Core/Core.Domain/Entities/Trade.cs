using Core.Domain.Abstraction;

namespace Core.Domain.Entities;

public record Trade : IEntity
{
    public int Id { get; init; }
    public int ClientId { get; init; }
    public string CurrencyFrom { get; init; }
    public string CurrencyTo { get; init; }
    public decimal ValueFrom { get; init; }
    public decimal ValueTo { get; init; }
    public decimal ExchangeRate { get; init; }
    public DateTime Timestamp { get; set; }
    public Client Client { get; init; } 
}
using Core.Domain.Abstraction;

namespace Core.Domain.Entities;

public record Client : IEntity
{
    public int Id { get; init; }
    public string IpAddress { get; init; }
    public ICollection<Trade> Trades { get; init; } = new List<Trade>();
}
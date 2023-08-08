using Core.Domain.Entities;

namespace Core.Domain.Abstraction.Repositories;

public interface ITradeRepository : IRepository<Trade>
{
    Task<int> CountTradesAsync(int clientId, DateTime fromDateTime);
}
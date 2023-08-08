using Core.Domain.Abstraction.Repositories;
using Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Shared.EntityFramework.Abstraction;

namespace Shared.EntityFramework.Repositories;

public class TradeRepository : RepositoryBase<Trade>, ITradeRepository
{
    public TradeRepository(IDbContext dbContext) : base(dbContext) { }

    public Task<int> CountTradesAsync(int clientId, DateTime fromDateTime) => _dbContext.Trades
        .CountAsync(t => t.ClientId == clientId && t.Timestamp >= fromDateTime.ToUniversalTime());
}
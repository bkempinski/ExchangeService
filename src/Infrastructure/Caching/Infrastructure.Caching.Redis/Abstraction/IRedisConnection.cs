using StackExchange.Redis;

namespace Infrastructure.Caching.Redis.Abstraction;

internal interface IRedisConnection
{
    IConnectionMultiplexer GetConnectionMultiplexer();
    Task<IConnectionMultiplexer> GetConnectionMultiplexerAsync();
}
using Infrastructure.Caching.Redis.Abstraction;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Extensions.Options;

namespace Infrastructure.Caching.Redis.Options;

internal class ConfigureRedisCacheOptions : IConfigureOptions<RedisCacheOptions>
{
    private readonly IRedisConnection _redisConnection;

    public ConfigureRedisCacheOptions(IRedisConnection redisConnection) => 
        _redisConnection = redisConnection;

    public void Configure(RedisCacheOptions options)
    {
        options.InstanceName = $"ExchangeService_{Environment.MachineName}";
        options.ConnectionMultiplexerFactory = () => _redisConnection.GetConnectionMultiplexerAsync();
    }
}
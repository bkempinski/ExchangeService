using Infrastructure.Caching.Redis.Abstraction;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace Infrastructure.Caching.Redis.Implementation;

internal class RedisConnection : IRedisConnection
{
    private readonly ILogger<RedisConnection> _logger;
    private readonly IConfiguration _configuration;

    private SemaphoreSlim _semaphore = new(1, 1);
    private IConnectionMultiplexer _connectionMultiplexer = null;

    public RedisConnection
        (
            ILogger<RedisConnection> logger,
            IConfiguration configuration
        ) => (_logger, _configuration)
            = (logger, configuration);

    public IConnectionMultiplexer GetConnectionMultiplexer()
    {
        _logger.LogDebug($"RedisConnection -> GetConnectionMultiplexer");

        if (_connectionMultiplexer != null)
            return _connectionMultiplexer;

        var redisConfiguration = GetRedisConfiguration();

        _logger.LogDebug($"RedisConnection -> New Redis connection: {redisConfiguration}");

        return _connectionMultiplexer = ConnectionMultiplexer.Connect(redisConfiguration);
    }

    public async Task<IConnectionMultiplexer> GetConnectionMultiplexerAsync()
    {
        _logger.LogDebug($"RedisConnection -> GetConnectionMultiplexerAsync");

        await _semaphore.WaitAsync();

        try
        {
            if (_connectionMultiplexer != null)
                return _connectionMultiplexer;

            var redisConfiguration = GetRedisConfiguration();

            _logger.LogDebug($"RedisConnection -> New Redis connection: {redisConfiguration}");

            return _connectionMultiplexer = await ConnectionMultiplexer.ConnectAsync(redisConfiguration);
        }
        finally
        {
            _semaphore.Release();
        }
    }

    private string GetRedisConfiguration()
    {
        var redisConfiguration = _configuration.GetConnectionString("RedisDistributedCache");

        if (string.IsNullOrEmpty(redisConfiguration))
            throw new Core.Domain.Exceptions.ArgumentNullException(nameof(redisConfiguration));

        return redisConfiguration;
    }
}
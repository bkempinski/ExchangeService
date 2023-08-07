using Infrastructure.Caching.Redis.Abstraction;
using Infrastructure.Caching.Redis.Implementation;
using Infrastructure.Caching.Redis.Options;
using Medallion.Threading;
using Medallion.Threading.Redis;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Caching.Redis;

public static class RedisCacheExtensions
{
    public static IServiceCollection AddRedisDistributedCache(this IServiceCollection services)
    {
        services.ConfigureOptions<ConfigureRedisCacheOptions>();

        services.AddSingleton<IRedisConnection, RedisConnection>();
        services.AddSingleton<IDistributedLockProvider>(services =>
        {
            var redisConnection = services.GetRequiredService<IRedisConnection>();
            var redisDatabase = redisConnection
                .GetConnectionMultiplexer()
                .GetDatabase();

            return new RedisDistributedSynchronizationProvider(redisDatabase);
        });

        // IDistributedCache implementation
        services.AddStackExchangeRedisCache(_ => { });

        return services;
    }
}
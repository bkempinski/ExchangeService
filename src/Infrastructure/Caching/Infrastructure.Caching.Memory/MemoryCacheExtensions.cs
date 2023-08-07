using Medallion.Threading;
using Medallion.Threading.WaitHandles;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Caching.Memory;

public static class MemoryCacheExtensions
{
    public static IServiceCollection AddInMemoryCache(this IServiceCollection services)
    {
        // IMemoryCache implementation
        services.AddMemoryCache();

        return services;
    }

    public static IServiceCollection AddInMemoryDistributedCache(this IServiceCollection services)
    {
        // Windows only  
        services.AddSingleton<IDistributedLockProvider, WaitHandleDistributedSynchronizationProvider>();

        // IDistributedCache implementation
        services.AddDistributedMemoryCache();

        return services;
    }
}
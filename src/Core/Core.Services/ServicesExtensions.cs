using Core.Services.Abstraction;
using Core.Services.Implementation;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Services;

public static class ServicesExtensions
{
    public static IServiceCollection AddCoreServices(this IServiceCollection services)
    {
        services.AddScoped<IExchangeService, ExchangeService>();
        services.AddScoped<ICacheService, CacheService>();

        return services;
    }
}
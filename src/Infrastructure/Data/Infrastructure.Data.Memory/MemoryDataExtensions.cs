using Core.Domain.Abstraction.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Shared.EntityFramework.Abstraction;
using Shared.EntityFramework.Repositories;

namespace Infrastructure.Data.Memory;

public static class MemoryDataExtensions
{
    public static IServiceCollection AddInMemoryDataStore(this IServiceCollection services)
    {
        services.AddSingleton<IDbContext, MemoryDbContext>();

        services.AddScoped<IClientRepository, ClientRepository>();
        services.AddScoped<ITradeRepository, TradeRepository>();

        return services;
    }
}
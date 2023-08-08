using Core.Domain.Abstraction.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Shared.EntityFramework.Abstraction;
using Shared.EntityFramework.Repositories;

namespace Infrastructure.Data.SqlServer;

public static class SqlServerDataExtensions
{
    public static IServiceCollection AddSqlServerDataStore(this IServiceCollection services)
    {
        services.AddSingleton<IDbContext, SqlServerDbContext>();

        services.AddScoped<IClientRepository, ClientRepository>();
        services.AddScoped<ITradeRepository, TradeRepository>();

        return services;
    }
}
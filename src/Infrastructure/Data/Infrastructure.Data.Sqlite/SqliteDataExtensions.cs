using Core.Domain.Abstraction.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Shared.EntityFramework.Abstraction;
using Shared.EntityFramework.Repositories;

namespace Infrastructure.Data.Sqlite;

public static class SqliteDataExtensions
{
    public static IServiceCollection AddSqliteDataStore(this IServiceCollection services)
    {
        services.AddSingleton<IDbContext, SqliteDbContext>();

        services.AddScoped<IClientRepository, ClientRepository>();
        services.AddScoped<ITradeRepository, TradeRepository>();

        return services;
    }
}
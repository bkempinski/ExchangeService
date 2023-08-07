﻿using Core.Services;
using Infrastructure.Caching.Memory;
using Infrastructure.Caching.Redis;
using Infrastructure.Providers.Fixer;
using Infrastructure.Providers.ExchangeRatesApi;
using Shared.Infrastructure.Exceptions;
using Serilog;

namespace App.ExchangeApi;

public static class ExchangeApiExtensions
{
    public static WebApplicationBuilder AddLogs(this WebApplicationBuilder builder)
    {
        var logger = new LoggerConfiguration()
            .ReadFrom.Configuration(builder.Configuration)
            .Enrich.FromLogContext()
#if DEBUG
            .WriteTo.Debug()
#endif
            .WriteTo.File(
                @"Logs\ExchangeService-.log",
                shared: true,
                rollingInterval: RollingInterval.Day,
                retainedFileTimeLimit: TimeSpan.FromDays(7))
            .CreateLogger();

        builder.Logging.ClearProviders();
        builder.Logging.AddSerilog(logger);

        return builder;
    }

    public static WebApplicationBuilder AddOptions(this WebApplicationBuilder builder)
    {
        //builder.Services.ConfigureOptions<ConfigureHostOptions>();

        return builder;
    }

    public static WebApplicationBuilder AddImplementations(this WebApplicationBuilder builder)
    {
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddDomainServices(builder.Configuration);

        return builder;
    }

    private static IServiceCollection AddDomainServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Core
        services.AddCoreServices();

        // App

        // Infrastructure
        services.AddInMemoryCache();

        var distributedCacheType = configuration.GetValue("DistributedCacheType", "Memory");

        switch (distributedCacheType)
        {
            // Memory - use for development
            case "Memory":
                services.AddInMemoryDistributedCache();
                break;
            // Redis - use for release
            case "Redis":
                services.AddRedisDistributedCache();
                break;
            default:
                throw new UnsupportedCacheException(distributedCacheType);
        }

        services.AddInfrastructureFixerExchangeRateProvider();
        services.AddInfrastructureExchangeRatesApiExchangeRateProvider();

        return services;
    }
}
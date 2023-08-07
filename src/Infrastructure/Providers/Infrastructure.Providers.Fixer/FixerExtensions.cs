using Core.Domain.Abstraction;
using Infrastructure.Providers.Fixer.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Shared.Polly;

namespace Infrastructure.Providers.Fixer;

public static class FixerExtensions
{
    public static IServiceCollection AddInfrastructureFixerExchangeRateProvider(this IServiceCollection services)
    {
        services.ConfigureOptions<ConfigureFixerOptions>();

        services
            .AddHttpClient<IExchangeRateProvider, FixerProvider>((services, client) =>
            {
                var fixerOptions = services.GetRequiredService<IOptions<FixerOptions>>();

                client.BaseAddress = new Uri(fixerOptions.Value.ApiBaseUrl);
            })
            .AddPolicyHandler(PollyExtensions.GetRetryPolicy())
            .AddPolicyHandler(PollyExtensions.GetCircuitBreakerPolicy());

        return services;
    }
}
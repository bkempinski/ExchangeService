using Core.Domain.Abstraction;
using Infrastructure.Providers.ExchangeRatesApi.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Shared.Polly;

namespace Infrastructure.Providers.ExchangeRatesApi;

public static class ExchangeRatesApiExtensions
{
    public static IServiceCollection AddInfrastructureExchangeRatesApiExchangeRateProvider(this IServiceCollection services)
    {
        services.ConfigureOptions<ConfigureExchangeRatesApiOptions>();

        services
            .AddHttpClient<IExchangeRateProvider, ExchangeRatesApiProvider>((services, client) =>
            {
                var fixerOptions = services.GetRequiredService<IOptions<ExchangeRatesApiOptions>>();

                client.BaseAddress = new Uri(fixerOptions.Value.ApiBaseUrl);
            })
            .AddPolicyHandler(PollyExtensions.GetRetryPolicy())
            .AddPolicyHandler(PollyExtensions.GetCircuitBreakerPolicy());

        return services;
    }
}
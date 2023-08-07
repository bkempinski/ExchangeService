using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace Infrastructure.Providers.ExchangeRatesApi.Options;

internal class ConfigureExchangeRatesApiOptions : IConfigureOptions<ExchangeRatesApiOptions>
{
    private readonly IConfiguration _configuration;

    public ConfigureExchangeRatesApiOptions(IConfiguration configuration) => 
        _configuration = configuration;

    public void Configure(ExchangeRatesApiOptions options) => 
        _configuration.Bind("ExchangeRateProviders:ExchangeRatesApi", options);
}
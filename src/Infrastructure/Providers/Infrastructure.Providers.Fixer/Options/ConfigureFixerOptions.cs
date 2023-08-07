using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace Infrastructure.Providers.Fixer.Options;

internal class ConfigureFixerOptions : IConfigureOptions<FixerOptions>
{
    private readonly IConfiguration _configuration;

    public ConfigureFixerOptions(IConfiguration configuration) => 
        _configuration = configuration;

    public void Configure(FixerOptions options) =>
        _configuration.Bind("ExchangeRateProviders:Fixer", options);
}
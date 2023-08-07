namespace Infrastructure.Providers.ExchangeRatesApi.Options;

public record ExchangeRatesApiOptions
{
    public string ApiBaseUrl { get; init; }
    public string ApiAccessKey { get; init; }
}
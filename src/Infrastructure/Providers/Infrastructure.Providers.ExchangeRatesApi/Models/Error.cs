namespace Infrastructure.Providers.ExchangeRatesApi.Models;

internal record Error
{
    public int Code { get; init; }
    public string Type { get; init; }
    public string Info { get; init; }
}
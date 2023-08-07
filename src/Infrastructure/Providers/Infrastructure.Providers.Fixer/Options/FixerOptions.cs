namespace Infrastructure.Providers.Fixer.Options;

public record FixerOptions
{
    public string ApiBaseUrl { get; init; }
    public string ApiAccessKey { get; init; }
}
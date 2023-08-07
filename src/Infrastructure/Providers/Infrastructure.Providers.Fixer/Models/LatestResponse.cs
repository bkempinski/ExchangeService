namespace Infrastructure.Providers.Fixer.Models;

internal record LatestResponse
{
    public bool Success { get; init; }
    public long Timestamp { get; init; }
    public string Base { get; init; }
    public DateOnly Date { get; init; }
    public IDictionary<string, decimal> Rates { get; init; }
    public Error Error { get; init; }
}
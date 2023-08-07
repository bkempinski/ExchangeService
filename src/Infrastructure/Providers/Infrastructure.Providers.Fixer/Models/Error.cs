namespace Infrastructure.Providers.Fixer.Models;

internal record Error
{
    public int Code { get; init; }
    public string Type { get; init; }
    public string Info { get; init; }
}
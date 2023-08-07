namespace Shared.Infrastructure.Exceptions;

public class UnsupportedCacheException : InfrastructureException
{
    public UnsupportedCacheException(string cacheType) : base($"Unsupported cache type: {cacheType}") { }
}
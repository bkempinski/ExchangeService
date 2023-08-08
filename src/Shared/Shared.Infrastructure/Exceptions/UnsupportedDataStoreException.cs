namespace Shared.Infrastructure.Exceptions;

public class UnsupportedDataStoreException : InfrastructureException
{
    public UnsupportedDataStoreException(string dataStoreType) : base($"Unsupported data store type: {dataStoreType}") { }
}
namespace Shared.Infrastructure.Exceptions;

public class ProviderException : InfrastructureException
{
    public ProviderException(string message) : base(message) { }
}
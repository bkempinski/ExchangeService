using Core.Domain.Exceptions;

namespace Shared.Infrastructure.Exceptions;

public abstract class InfrastructureException : DomainException
{
    public InfrastructureException(string message) : base(message) { }

    public InfrastructureException(string message, Exception innerException) : base(message, innerException) { }
}
namespace Core.Domain.Exceptions;

public class ArgumentNullException : DomainException
{
    public ArgumentNullException(string paramName) : base($"Parameter cannot be empty - {paramName}") { }
}
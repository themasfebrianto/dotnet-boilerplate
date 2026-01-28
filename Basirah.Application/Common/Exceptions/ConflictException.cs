namespace Basirah.Application.Common.Exceptions;

/// <summary>
/// Thrown when attempting to create a duplicate resource.
/// Maps to HTTP 409 Conflict.
/// </summary>
public class ConflictException : AppException
{
    public override int StatusCode => 409;

    public ConflictException(string message)
        : base(message) { }

    public ConflictException(string entity, string identifier)
        : base($"{entity} with '{identifier}' already exists.") { }
}

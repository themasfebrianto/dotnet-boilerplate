namespace Basirah.Application.Common.Exceptions;

/// <summary>
/// Thrown when a requested entity is not found.
/// Maps to HTTP 404 Not Found.
/// </summary>
public class NotFoundException : AppException
{
    public override int StatusCode => 404;

    public NotFoundException(string entity, object id)
        : base($"{entity} with ID '{id}' was not found.") { }

    public NotFoundException(string message)
        : base(message) { }
}

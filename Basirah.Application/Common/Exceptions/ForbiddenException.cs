namespace Basirah.Application.Common.Exceptions;

/// <summary>
/// Thrown when user is authenticated but lacks permission for the action.
/// Maps to HTTP 403 Forbidden.
/// </summary>
public class ForbiddenException : AppException
{
    public override int StatusCode => 403;

    public ForbiddenException(string message = "You do not have permission to perform this action.")
        : base(message) { }
}

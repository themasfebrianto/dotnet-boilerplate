namespace Basirah.Application.Common.Exceptions;

/// <summary>
/// Thrown when authentication fails (invalid credentials, expired token, etc.).
/// Maps to HTTP 401 Unauthorized.
/// </summary>
public class UnauthorizedException : AppException
{
    public override int StatusCode => 401;

    public UnauthorizedException(string message = "Invalid credentials.")
        : base(message) { }
}

namespace Basirah.Application.Common.Exceptions;

/// <summary>
/// Base exception for all application-level exceptions.
/// Middleware maps these to HTTP responses automatically.
/// </summary>
public abstract class AppException : Exception
{
    /// <summary>
    /// HTTP status code to return when this exception is thrown.
    /// </summary>
    public abstract int StatusCode { get; }

    /// <summary>
    /// Error code for client-side handling (derived from class name by default).
    /// </summary>
    public virtual string ErrorCode => GetType().Name.Replace("Exception", "");

    protected AppException(string message) : base(message) { }
    protected AppException(string message, Exception inner) : base(message, inner) { }
}

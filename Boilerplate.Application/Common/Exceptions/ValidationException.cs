namespace Boilerplate.Application.Common.Exceptions;

/// <summary>
/// Thrown when input validation fails or a business rule is violated.
/// Maps to HTTP 400 Bad Request.
/// </summary>
public class ValidationException : AppException
{
    public override int StatusCode => 400;

    /// <summary>
    /// Validation errors keyed by field name.
    /// </summary>
    public IDictionary<string, string[]>? Errors { get; }

    public ValidationException(string message)
        : base(message) { }

    public ValidationException(IDictionary<string, string[]> errors)
        : base("One or more validation errors occurred.")
    {
        Errors = errors;
    }
}

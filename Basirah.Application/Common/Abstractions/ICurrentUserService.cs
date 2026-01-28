namespace Basirah.Application.Common.Abstractions;

/// <summary>
/// Provides access to the current authenticated user's information.
/// Injected into services that need user context.
/// </summary>
public interface ICurrentUserService
{
    /// <summary>
    /// The current user's ID, or null if not authenticated.
    /// </summary>
    Guid? UserId { get; }

    /// <summary>
    /// The current user's email, or null if not authenticated.
    /// </summary>
    string? Email { get; }

    /// <summary>
    /// The roles assigned to the current user.
    /// </summary>
    IEnumerable<string> Roles { get; }

    /// <summary>
    /// Whether the current request is from an authenticated user.
    /// </summary>
    bool IsAuthenticated { get; }
}

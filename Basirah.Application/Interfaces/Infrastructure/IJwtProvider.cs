using Basirah.Domain.Entities;

namespace Basirah.Application.Interfaces.Infrastructure;

/// <summary>
/// JWT token provider interface.
/// Implemented in Infrastructure layer.
/// </summary>
public interface IJwtProvider
{
    /// <summary>
    /// Generate an access token for the given user.
    /// </summary>
    string GenerateAccessToken(User user);

    /// <summary>
    /// Generate a cryptographically secure refresh token.
    /// </summary>
    string GenerateRefreshToken();
}

using Basirah.Domain.Entities;

namespace Basirah.Application.Interfaces.Repositories;

/// <summary>
/// Refresh token repository interface for data access.
/// </summary>
public interface IRefreshTokenRepository
{
    Task<RefreshToken?> GetByTokenAsync(string token);
    Task<RefreshToken?> GetByUserIdAsync(Guid userId);
    Task CreateAsync(RefreshToken refreshToken);
    Task UpdateAsync(RefreshToken refreshToken);
    Task DeleteByUserIdAsync(Guid userId);
    Task RevokeAsync(string token, string? revokedByIp = null);

    /// <summary>
    /// Creates or updates the refresh token for a user (upsert pattern).
    /// </summary>
    Task UpsertAsync(Guid userId, string token, DateTime expiresAt, string? createdByIp = null);
}

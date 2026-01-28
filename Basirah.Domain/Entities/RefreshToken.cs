using Basirah.Domain.Common;

namespace Basirah.Domain.Entities;

/// <summary>
/// Refresh token entity for JWT token refresh flows.
/// Does not implement ISoftDeletable or IAuditable - tokens are simply deleted when revoked.
/// </summary>
public class RefreshToken : BaseEntity
{
    public Guid UserId { get; set; }
    public string Token { get; set; } = null!;
    public DateTime ExpiresAt { get; set; }
    public string? CreatedByIp { get; set; }
    public DateTime? RevokedAt { get; set; }
    public string? RevokedByIp { get; set; }
    public string? ReplacedByToken { get; set; }

    // Computed properties
    public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
    public bool IsActive => RevokedAt == null && !IsExpired;

    // Navigation property
    public User? User { get; set; }
}

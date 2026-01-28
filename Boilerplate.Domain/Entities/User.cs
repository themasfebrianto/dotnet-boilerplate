using Boilerplate.Domain.Common;
using Boilerplate.Domain.Common.Interfaces;

namespace Boilerplate.Domain.Entities;

/// <summary>
/// User entity representing an authenticated user in the system.
/// Implements IAuditable for user tracking.
/// </summary>
public class User : BaseEntity, IAuditable
{
    // Core user properties
    public string Email { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;
    public string? FullName { get; set; }
    public string? Bio { get; set; }
    public string? AvatarUrl { get; set; }
    public bool IsEmailVerified { get; set; } = false;
    public Guid? RoleId { get; set; }
    public DateTime? LastLoginAt { get; set; }

    // IAuditable implementation
    public Guid? CreatedBy { get; set; }
    public Guid? UpdatedBy { get; set; }

    // Navigation property
    public Role? Role { get; set; }
}

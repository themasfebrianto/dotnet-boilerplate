using Boilerplate.Domain.Common;
using Boilerplate.Domain.Common.Interfaces;

namespace Boilerplate.Domain.Entities;

/// <summary>
/// Role entity for role-based access control (RBAC).
/// Implements ISoftDeletable for soft delete support.
/// </summary>
public class Role : BaseEntity, ISoftDeletable
{
    // Core role properties
    public string Name { get; set; } = null!;
    public string? Description { get; set; }

    // ISoftDeletable implementation
    public DateTime? DeletedAt { get; set; }
    public Guid? DeletedBy { get; set; }

    // Navigation property
    public ICollection<User> Users { get; set; } = [];
}

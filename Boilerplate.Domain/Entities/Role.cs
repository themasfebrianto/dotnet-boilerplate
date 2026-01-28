using Boilerplate.Domain.Common;

namespace Boilerplate.Domain.Entities;

/// <summary>
/// Role entity for role-based access control (RBAC).
/// </summary>
public class Role : BaseEntity
{
    // Core role properties
    public string Name { get; set; } = null!;
    public string? Description { get; set; }

    // Navigation property
    public ICollection<User> Users { get; set; } = [];
}

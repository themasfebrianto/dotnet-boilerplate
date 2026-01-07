namespace Boilerplate.Domain.Common.Interfaces;

/// <summary>
/// Marks an entity as soft-deletable.
/// Implement this interface to enable soft delete functionality.
/// </summary>
public interface ISoftDeletable
{
    DateTime? DeletedAt { get; set; }
    Guid? DeletedBy { get; set; }
    
    /// <summary>
    /// Computed property indicating if the entity has been soft-deleted.
    /// </summary>
    bool IsDeleted => DeletedAt.HasValue;
}

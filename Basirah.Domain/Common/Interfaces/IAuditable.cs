namespace Basirah.Domain.Common.Interfaces;

/// <summary>
/// Marks an entity as auditable with user tracking.
/// Implement this interface to track who created/updated the entity.
/// </summary>
public interface IAuditable
{
    Guid? CreatedBy { get; set; }
    Guid? UpdatedBy { get; set; }
}

using Boilerplate.Application.Common.Abstractions;

namespace Boilerplate.Application.DTOs.Role;

/// <summary>
/// Response DTO for role data.
/// </summary>
public class RoleResponseDto : IHasId
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
}

using System.ComponentModel.DataAnnotations;

namespace Boilerplate.Application.DTOs.Role;

/// <summary>
/// Request DTO for creating or updating a role.
/// </summary>
public class RoleRequestDto
{
    [Required]
    [MaxLength(50)]
    public string Name { get; set; } = null!;

    [MaxLength(200)]
    public string? Description { get; set; }
}

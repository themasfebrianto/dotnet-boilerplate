using System.ComponentModel.DataAnnotations;

namespace Boilerplate.Application.DTOs.User;

/// <summary>
/// Request DTO for creating or updating a user.
/// </summary>
public class UserRequestDto
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = null!;

    [Required]
    [MinLength(6)]
    public string Password { get; set; } = null!;

    [MaxLength(100)]
    public string? FullName { get; set; }

    [MaxLength(500)]
    public string? Bio { get; set; }

    public Guid? RoleId { get; set; }
}

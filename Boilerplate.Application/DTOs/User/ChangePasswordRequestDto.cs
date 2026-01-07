using System.ComponentModel.DataAnnotations;

namespace Boilerplate.Application.DTOs.User;

/// <summary>
/// Request DTO for changing password.
/// </summary>
public class ChangePasswordRequestDto
{
    [Required]
    public string CurrentPassword { get; set; } = null!;

    [Required]
    [MinLength(6)]
    public string NewPassword { get; set; } = null!;
}

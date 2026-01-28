using System.ComponentModel.DataAnnotations;

namespace Basirah.Application.DTOs.Auth;

/// <summary>
/// Request DTO for user login.
/// </summary>
public class LoginRequestDto
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = null!;

    [Required]
    [MinLength(6)]
    public string Password { get; set; } = null!;
}

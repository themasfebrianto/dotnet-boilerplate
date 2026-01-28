using System.ComponentModel.DataAnnotations;

namespace Basirah.Application.DTOs.Auth;

/// <summary>
/// Request DTO for refreshing access token.
/// </summary>
public class RefreshTokenRequestDto
{
    [Required]
    public string RefreshToken { get; set; } = null!;
}

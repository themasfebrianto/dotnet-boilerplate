namespace Boilerplate.Application.DTOs.Auth;

/// <summary>
/// Response DTO for successful authentication.
/// </summary>
public class LoginResponseDto
{
    public string AccessToken { get; set; } = null!;
    public string RefreshToken { get; set; } = null!;
    public DateTime AccessTokenExpires { get; set; }
    public DateTime RefreshTokenExpires { get; set; }
}

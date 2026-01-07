namespace Boilerplate.Application.DTOs.User;

/// <summary>
/// Response DTO for user data.
/// </summary>
public class UserResponseDto
{
    public Guid Id { get; set; }
    public string Email { get; set; } = null!;
    public string? FullName { get; set; }
    public string? Bio { get; set; }
    public string? AvatarUrl { get; set; }
    public bool IsEmailVerified { get; set; }
    public Guid? RoleId { get; set; }
    public string? RoleName { get; set; }
    public DateTime? LastLoginAt { get; set; }
    public DateTime CreatedAt { get; set; }
}

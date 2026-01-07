using Boilerplate.Application.DTOs.Auth;
using Boilerplate.Application.DTOs.User;

namespace Boilerplate.Application.Interfaces.Services;

/// <summary>
/// Authentication service interface.
/// All methods throw exceptions on failure (exception-driven flow).
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// Authenticate user with email/password.
    /// </summary>
    /// <exception cref="Common.Exceptions.UnauthorizedException">Invalid credentials.</exception>
    Task<LoginResponseDto> LoginAsync(LoginRequestDto request);

    /// <summary>
    /// Register a new user.
    /// </summary>
    /// <exception cref="Common.Exceptions.ConflictException">Email already registered.</exception>
    Task<LoginResponseDto> RegisterAsync(UserRequestDto request);

    /// <summary>
    /// Refresh access token using a valid refresh token.
    /// </summary>
    /// <exception cref="Common.Exceptions.UnauthorizedException">Invalid or expired refresh token.</exception>
    Task<LoginResponseDto> RefreshTokenAsync(string refreshToken);

    /// <summary>
    /// Revoke a refresh token (logout).
    /// </summary>
    Task RevokeTokenAsync(string refreshToken);
}

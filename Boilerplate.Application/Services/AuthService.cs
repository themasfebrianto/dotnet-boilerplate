using Boilerplate.Application.Common.Abstractions;
using Boilerplate.Application.Common.Exceptions;
using Boilerplate.Application.Common.Settings;
using Boilerplate.Application.DTOs.Auth;
using Boilerplate.Application.DTOs.User;
using Boilerplate.Application.Interfaces.Infrastructure;
using Boilerplate.Application.Interfaces.Repositories;
using Boilerplate.Application.Interfaces.Services;
using Boilerplate.Application.Mappings;
using Boilerplate.Domain.Entities;
using Microsoft.Extensions.Options;

namespace Boilerplate.Application.Services;

/// <summary>
/// Authentication service implementation.
/// Follows exception-driven flow - throws on failure, never returns null.
/// </summary>
public class AuthService(
    IUserRepository userRepository,
    IRefreshTokenRepository refreshTokenRepository,
    IJwtProvider jwtProvider,
    IOptions<JwtSettings> jwtSettings) : IAuthService
{
    private readonly JwtSettings _jwtSettings = jwtSettings.Value;

    public async Task<LoginResponseDto> LoginAsync(LoginRequestDto request)
    {
        var user = await userRepository.GetByEmailAsync(request.Email)
            ?? throw new UnauthorizedException();

        if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            throw new UnauthorizedException();

        return await GenerateTokensAsync(user);
    }

    public async Task<LoginResponseDto> RegisterAsync(UserRequestDto request)
    {
        if (await userRepository.EmailExistsAsync(request.Email))
            throw new ConflictException("Email already registered.");

        var user = request.ToEntity();
        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
        user.CreatedAt = DateTime.UtcNow;

        var createdUser = await userRepository.CreateAsync(user);
        return await GenerateTokensAsync(createdUser);
    }

    public async Task<LoginResponseDto> RefreshTokenAsync(string refreshToken)
    {
        var storedToken = await refreshTokenRepository.GetByTokenAsync(refreshToken)
            ?? throw new UnauthorizedException("Invalid refresh token.");

        if (!storedToken.IsActive)
            throw new UnauthorizedException("Refresh token has expired or been revoked.");

        var user = await userRepository.GetByIdAsync(storedToken.UserId)
            ?? throw new UnauthorizedException("User not found.");

        return await GenerateTokensAsync(user);
    }

    public async Task RevokeTokenAsync(string refreshToken)
    {
        await refreshTokenRepository.RevokeAsync(refreshToken);
    }

    private async Task<LoginResponseDto> GenerateTokensAsync(User user)
    {
        var accessToken = jwtProvider.GenerateAccessToken(user);
        var newRefreshToken = jwtProvider.GenerateRefreshToken();
        var refreshExpiry = DateTime.UtcNow.Add(_jwtSettings.RefreshTokenExpiration);

        await refreshTokenRepository.UpsertAsync(user.Id, newRefreshToken, refreshExpiry);

        // Update last login
        user.LastLoginAt = DateTime.UtcNow;
        await userRepository.UpdateAsync(user);

        return new LoginResponseDto
        {
            AccessToken = accessToken,
            RefreshToken = newRefreshToken,
            AccessTokenExpires = DateTime.UtcNow.Add(_jwtSettings.Expiration),
            RefreshTokenExpires = refreshExpiry
        };
    }
}

namespace Basirah.Application.Common.Settings;

/// <summary>
/// JWT authentication configuration.
/// Bound to "JwtSettings" section in appsettings.json.
/// </summary>
public class JwtSettings
{
    public const string SectionName = "JwtSettings";

    public string IssuerSigningKey { get; set; } = null!;
    public string ValidIssuer { get; set; } = null!;
    public string ValidAudience { get; set; } = null!;
    public TimeSpan Expiration { get; set; } = TimeSpan.FromMinutes(15);
    public TimeSpan RefreshTokenExpiration { get; set; } = TimeSpan.FromDays(7);
}

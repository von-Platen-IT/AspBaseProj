namespace AspBaseProj.Presentation.Options;

/// <summary>
/// JWT token configuration loaded from the "Jwt" configuration section.
/// </summary>
public sealed class JwtOptions
{
    public string Issuer { get; init; } = string.Empty;
    public string Audience { get; init; } = string.Empty;
    public string SecretKey { get; init; } = string.Empty;
    public int ExpiryMinutes { get; init; } = 60;
}

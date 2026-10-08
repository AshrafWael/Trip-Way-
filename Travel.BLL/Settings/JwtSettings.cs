namespace Travel.BLL.Settings;

/// <summary>
/// Bound from the "JwtSettings" configuration section. Secret comes from
/// dotnet user-secrets / environment variables in every environment --
/// it is never given a default value here.
/// </summary>
public class JwtSettings
{
    public const string SectionName = "JwtSettings";

    public string Secret { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public int ExpiryMinutes { get; set; } = 60;
}

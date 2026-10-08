namespace Travel.DAL.Data.Seed;

/// <summary>
/// Bound from the "SeedAdmin" configuration section, which in turn comes
/// from dotnet user-secrets / environment variables in every environment
/// — never from a literal value in source.
/// </summary>
public class SeedAdminOptions
{
    public const string SectionName = "SeedAdmin";

    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string FullName { get; set; } = "Platform Administrator";
}

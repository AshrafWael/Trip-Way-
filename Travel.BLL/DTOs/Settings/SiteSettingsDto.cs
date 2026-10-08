namespace Travel.BLL.DTOs.Settings;

public class SiteSettingsDto
{
    public string HeroTitle { get; set; } = string.Empty;
    public string ArabicHeroTitle { get; set; } = string.Empty;
    public string HeroSubtitle { get; set; } = string.Empty;
    public string ArabicHeroSubtitle { get; set; } = string.Empty;
    public string? HeroImageUrl { get; set; }

    public string ContactPhone { get; set; } = string.Empty;
    public string ContactEmail { get; set; } = string.Empty;
    public string ContactAddress { get; set; } = string.Empty;
    public string ArabicContactAddress { get; set; } = string.Empty;
    public string ContactHours { get; set; } = string.Empty;
    public string ArabicContactHours { get; set; } = string.Empty;
    public string? WhatsAppNumber { get; set; }
}

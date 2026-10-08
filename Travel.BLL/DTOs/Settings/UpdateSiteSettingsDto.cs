using System.ComponentModel.DataAnnotations;

namespace Travel.BLL.DTOs.Settings;

public class UpdateSiteSettingsDto
{
    [Required, MaxLength(200), Display(Name = "HeroTitleLabel")]
    public string HeroTitle { get; set; } = string.Empty;

    [Required, MaxLength(200), Display(Name = "ArabicHeroTitle")]
    public string ArabicHeroTitle { get; set; } = string.Empty;

    [Required, MaxLength(500), Display(Name = "HeroSubtitleLabel")]
    public string HeroSubtitle { get; set; } = string.Empty;

    [Required, MaxLength(500), Display(Name = "ArabicHeroSubtitle")]
    public string ArabicHeroSubtitle { get; set; } = string.Empty;

    [MaxLength(500), Display(Name = "HeroImageUrl")]
    public string? HeroImageUrl { get; set; }

    [Required, MaxLength(50), Display(Name = "ContactPhone")]
    public string ContactPhone { get; set; } = string.Empty;

    [Required, EmailAddress, MaxLength(200), Display(Name = "ContactEmail")]
    public string ContactEmail { get; set; } = string.Empty;

    [Required, MaxLength(300), Display(Name = "ContactAddress")]
    public string ContactAddress { get; set; } = string.Empty;

    [Required, MaxLength(300), Display(Name = "ArabicContactAddress")]
    public string ArabicContactAddress { get; set; } = string.Empty;

    [Required, MaxLength(200), Display(Name = "ContactHours")]
    public string ContactHours { get; set; } = string.Empty;

    [Required, MaxLength(200), Display(Name = "ArabicContactHours")]
    public string ArabicContactHours { get; set; } = string.Empty;

    [MaxLength(20), Display(Name = "WhatsAppNumber")]
    public string? WhatsAppNumber { get; set; }
}

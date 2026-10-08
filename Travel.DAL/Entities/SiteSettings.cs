namespace Travel.DAL.Entities;

/// <summary>
/// A single-row settings table for content the admin can edit without a
/// deploy -- currently just the homepage hero. Always exactly one row
/// (Id = 1), seeded with the site's original hero copy so nothing
/// changes visually until an admin edits it.
/// </summary>
public class SiteSettings
{
    public int Id { get; set; }

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

    /// <summary>
    /// Digits only, with country code, no "+" or spaces (WhatsApp's
    /// wa.me link format expects e.g. "201234567890"). Booking pages
    /// use this to open a pre-filled WhatsApp chat with customer
    /// service instead of collecting payment on-site.
    /// </summary>
    public string? WhatsAppNumber { get; set; }
}

using System.Globalization;
using Travel.BLL.DTOs.Blog;
using Travel.BLL.DTOs.Destinations;
using Travel.BLL.DTOs.Hotels;
using Travel.BLL.DTOs.Packages;
using Travel.DAL.Entities;

namespace Travel.Web.Extensions;

/// <summary>
/// Every content entity in this project already stores both an English
/// and an Arabic copy of its text (Name/ArabicName, Title/ArabicTitle,
/// Description/ArabicDescription, Content/ArabicContent) -- see
/// Travel.DAL.Entities and the matching BLL DTOs. Views were only ever
/// reading the Arabic* fields though, so switching the UI language via
/// the navbar toggle changed labels/buttons but not the actual
/// destination/package/blog content. These helpers pick whichever copy
/// matches the current UI culture so views localize automatically.
/// </summary>
public static class LocalizedContentExtensions
{
    public static bool IsArabic =>
        CultureInfo.CurrentUICulture.Name.StartsWith("ar", StringComparison.OrdinalIgnoreCase);

    public static string L(this DestinationDto d) => IsArabic ? d.ArabicName : d.Name;
    public static string LDescription(this DestinationDto d) => IsArabic ? d.ArabicDescription : d.Description;

    // PackageDetailsDto : PackageDto, so this also covers list/details titles.
    public static string L(this PackageDto p) => IsArabic ? p.ArabicTitle : p.Title;
    public static string LDescription(this PackageDetailsDto p) => IsArabic ? p.ArabicDescription : p.Description;

    public static string L(this BlogPostDto b) => IsArabic ? b.ArabicTitle : b.Title;
    public static string LContent(this BlogPostDto b) => IsArabic ? b.ArabicContent : b.Content;
    public static string? LSubtitle(this BlogPostDto b) => IsArabic ? b.ArabicSubtitle : b.Subtitle;

    // HotelDetailsDto : HotelDto, so this also covers list/details titles.
    public static string L(this HotelDto h) => IsArabic ? h.ArabicName : h.Name;
    public static string LDescription(this HotelDetailsDto h) => IsArabic ? h.ArabicDescription : h.Description;
    public static string LAddress(this HotelDetailsDto h) => IsArabic ? h.ArabicAddress : h.Address;

    /// <summary>Localized display text for a booking status badge.</summary>
    public static string LStatus(this BookingStatus status, Microsoft.Extensions.Localization.IStringLocalizer localizer) =>
        localizer[$"{status}"];
}

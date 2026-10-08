namespace Travel.BLL.DTOs.Packages;

/// <summary>Lightweight shape used for listing/search results.</summary>
public class PackageDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string ArabicTitle { get; set; } = string.Empty;
    public int DestinationId { get; set; }
    public string DestinationName { get; set; } = string.Empty;
    public int DurationDays { get; set; }
    public int DurationNights { get; set; }
    public decimal? Price { get; set; }
    public decimal? DiscountPrice { get; set; }
    public int AvailableSeats { get; set; }
    public string? MainImageUrl { get; set; }
    public string? VideoUrl { get; set; }
    public bool IsFeatured { get; set; }
    public bool IsActive { get; set; }
    public double AverageRating { get; set; }
    public int ReviewCount { get; set; }
}

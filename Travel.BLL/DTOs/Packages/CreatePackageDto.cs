using System.ComponentModel.DataAnnotations;

namespace Travel.BLL.DTOs.Packages;

public class CreatePackageDto
{
    [Required, MaxLength(200), Display(Name = "Field_Title")]
    public string Title { get; set; } = string.Empty;

    [Required, MaxLength(200), Display(Name = "ArabicTitle")]
    public string ArabicTitle { get; set; } = string.Empty;

    [Required, MaxLength(4000), Display(Name = "Description")]
    public string Description { get; set; } = string.Empty;

    [Required, MaxLength(4000), Display(Name = "ArabicDescription")]
    public string ArabicDescription { get; set; } = string.Empty;

    [Required, Display(Name = "DestinationId")]
    public int DestinationId { get; set; }

    [Range(1, 60), Display(Name = "DurationDays")]
    public int DurationDays { get; set; }

    [Range(0, 59), Display(Name = "DurationNights")]
    public int DurationNights { get; set; }

    [Range(0, double.MaxValue), Display(Name = "Price")]
    public decimal? Price { get; set; }

    [Display(Name = "DiscountPrice")]
    public decimal? DiscountPrice { get; set; }

    [Range(1, 500), Display(Name = "MaxTravelers")]
    public int MaxTravelers { get; set; }

    [Range(0, 500), Display(Name = "AvailableSeats")]
    public int AvailableSeats { get; set; }

    [MaxLength(500), Display(Name = "MainImageUrl")]
    public string? MainImageUrl { get; set; }

    [MaxLength(500), Display(Name = "VideoUrl")]
    public string? VideoUrl { get; set; }

    [Display(Name = "IsFeatured")]
    public bool IsFeatured { get; set; }

    public List<string> ImageUrls { get; set; } = new();
}

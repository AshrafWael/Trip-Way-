using System.ComponentModel.DataAnnotations;

namespace Travel.BLL.DTOs.Hotels;

public class CreateHotelDto
{
    [Required, MaxLength(200), Display(Name = "Name")]
    public string Name { get; set; } = string.Empty;

    [Required, MaxLength(200), Display(Name = "ArabicName")]
    public string ArabicName { get; set; } = string.Empty;

    [Required, MaxLength(4000), Display(Name = "Description")]
    public string Description { get; set; } = string.Empty;

    [Required, MaxLength(4000), Display(Name = "ArabicDescription")]
    public string ArabicDescription { get; set; } = string.Empty;

    [Required, Display(Name = "DestinationId")]
    public int DestinationId { get; set; }

    [Required, MaxLength(300), Display(Name = "Address")]
    public string Address { get; set; } = string.Empty;

    [Required, MaxLength(300), Display(Name = "ArabicAddress")]
    public string ArabicAddress { get; set; } = string.Empty;

    [Range(1, 5), Display(Name = "StarRating")]
    public int StarRating { get; set; } = 3;

    [Range(0, double.MaxValue), Display(Name = "PricePerNight")]
    public decimal? PricePerNight { get; set; }

    [Display(Name = "DiscountPricePerNight")]
    public decimal? DiscountPricePerNight { get; set; }

    [Range(1, 2000), Display(Name = "TotalRooms")]
    public int TotalRooms { get; set; }

    [Range(0, 2000), Display(Name = "AvailableRooms")]
    public int AvailableRooms { get; set; }

    [MaxLength(500), Display(Name = "MainImageUrl")]
    public string? MainImageUrl { get; set; }

    [Display(Name = "IsFeatured")]
    public bool IsFeatured { get; set; }

    public List<string> ImageUrls { get; set; } = new();
}

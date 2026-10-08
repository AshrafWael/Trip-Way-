namespace Travel.BLL.DTOs.Hotels;

/// <summary>Lightweight shape used for listing/search results.</summary>
public class HotelDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string ArabicName { get; set; } = string.Empty;
    public int DestinationId { get; set; }
    public string DestinationName { get; set; } = string.Empty;
    public int StarRating { get; set; }
    public decimal? PricePerNight { get; set; }
    public decimal? DiscountPricePerNight { get; set; }
    public int TotalRooms { get; set; }
    public int AvailableRooms { get; set; }
    public string? MainImageUrl { get; set; }
    public bool IsFeatured { get; set; }
    public bool IsActive { get; set; }
}

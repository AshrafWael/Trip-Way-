namespace Travel.DAL.Entities;

public class Hotel
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;
    public string ArabicName { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;
    public string ArabicDescription { get; set; } = string.Empty;

    public int DestinationId { get; set; }
    public Destination Destination { get; set; } = null!;

    public string Address { get; set; } = string.Empty;
    public string ArabicAddress { get; set; } = string.Empty;

    public int StarRating { get; set; }

    public decimal? PricePerNight { get; set; }
    public decimal? DiscountPricePerNight { get; set; }

    public int TotalRooms { get; set; }
    public int AvailableRooms { get; set; }

    public string? MainImageUrl { get; set; }

    public bool IsFeatured { get; set; }
    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    // Navigation
    public ICollection<HotelImage> Images { get; set; } = new List<HotelImage>();
    public ICollection<HotelBooking> Bookings { get; set; } = new List<HotelBooking>();
}

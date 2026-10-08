namespace Travel.DAL.Entities;

public class TravelPackage
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;
    public string ArabicTitle { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;
    public string ArabicDescription { get; set; } = string.Empty;

    public int DestinationId { get; set; }
    public Destination Destination { get; set; } = null!;

    public int DurationDays { get; set; }
    public int DurationNights { get; set; }

    public decimal? Price { get; set; }
    public decimal? DiscountPrice { get; set; }

    public int MaxTravelers { get; set; }
    public int AvailableSeats { get; set; }

    public string? MainImageUrl { get; set; }
    public string? VideoUrl { get; set; }

    public bool IsFeatured { get; set; }
    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    // Navigation
    public ICollection<PackageImage> Images { get; set; } = new List<PackageImage>();
    public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    public ICollection<Wishlist> WishlistedBy { get; set; } = new List<Wishlist>();
    public ICollection<Review> Reviews { get; set; } = new List<Review>();
}

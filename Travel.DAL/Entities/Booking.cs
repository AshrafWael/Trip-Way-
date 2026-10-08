namespace Travel.DAL.Entities;

public class Booking
{
    public int Id { get; set; }

    public string UserId { get; set; } = string.Empty;
    public ApplicationUser User { get; set; } = null!;

    public int TravelPackageId { get; set; }
    public TravelPackage TravelPackage { get; set; } = null!;

    public DateTime BookingDate { get; set; } = DateTime.UtcNow;
    public DateTime TravelDate { get; set; }

    public int NumberOfTravelers { get; set; }
    public decimal TotalPrice { get; set; }

    public BookingStatus Status { get; set; } = BookingStatus.Pending;

    public string? SpecialRequests { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

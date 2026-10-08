namespace Travel.DAL.Entities;

public class HotelBooking
{
    public int Id { get; set; }

    public string UserId { get; set; } = string.Empty;
    public ApplicationUser User { get; set; } = null!;

    public int HotelId { get; set; }
    public Hotel Hotel { get; set; } = null!;

    public DateTime BookingDate { get; set; } = DateTime.UtcNow;
    public DateTime CheckInDate { get; set; }
    public DateTime CheckOutDate { get; set; }

    public int NumberOfRooms { get; set; }
    public int NumberOfGuests { get; set; }
    public decimal TotalPrice { get; set; }

    public BookingStatus Status { get; set; } = BookingStatus.Pending;

    public string? SpecialRequests { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

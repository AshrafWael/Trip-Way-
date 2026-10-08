using Travel.DAL.Entities;

namespace Travel.BLL.DTOs.Hotels;

public class HotelBookingDto
{
    public int Id { get; set; }
    public int HotelId { get; set; }
    public string HotelName { get; set; } = string.Empty;
    public string? HotelImageUrl { get; set; }
    public DateTime BookingDate { get; set; }
    public DateTime CheckInDate { get; set; }
    public DateTime CheckOutDate { get; set; }
    public int NumberOfRooms { get; set; }
    public int NumberOfGuests { get; set; }
    public decimal TotalPrice { get; set; }
    public BookingStatus Status { get; set; }
    public string? SpecialRequests { get; set; }
}

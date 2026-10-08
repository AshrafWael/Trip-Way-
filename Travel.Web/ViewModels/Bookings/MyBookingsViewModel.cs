using Travel.BLL.DTOs.Bookings;
using Travel.BLL.DTOs.Hotels;

namespace Travel.Web.ViewModels.Bookings;

public class MyBookingsViewModel
{
    public IReadOnlyList<BookingDto> PackageBookings { get; set; } = Array.Empty<BookingDto>();
    public IReadOnlyList<HotelBookingDto> HotelBookings { get; set; } = Array.Empty<HotelBookingDto>();
    public string? WhatsAppNumber { get; set; }
}

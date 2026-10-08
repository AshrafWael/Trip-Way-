using Travel.BLL.DTOs.Bookings;
using Travel.BLL.DTOs.Hotels;

namespace Travel.BLL.DTOs.Admin;

public class DashboardStatsDto
{
    public int TotalUsers { get; set; }
    public int TotalBookings { get; set; }
    public int TotalPackages { get; set; }
    public int TotalDestinations { get; set; }
    public int TotalHotels { get; set; }
    public int TotalHotelBookings { get; set; }
    public decimal Revenue { get; set; }
    public int PendingBookings { get; set; }
    public int PendingHotelBookings { get; set; }
    public List<BookingDto> RecentBookings { get; set; } = new();
    public List<HotelBookingDto> RecentHotelBookings { get; set; } = new();
    public List<UserSummaryDto> RecentUsers { get; set; } = new();
}

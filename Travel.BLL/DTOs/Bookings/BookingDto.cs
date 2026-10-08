using Travel.DAL.Entities;

namespace Travel.BLL.DTOs.Bookings;

public class BookingDto
{
    public int Id { get; set; }
    public int TravelPackageId { get; set; }
    public string PackageTitle { get; set; } = string.Empty;
    public string? PackageImageUrl { get; set; }
    public DateTime BookingDate { get; set; }
    public DateTime TravelDate { get; set; }
    public int NumberOfTravelers { get; set; }
    public decimal TotalPrice { get; set; }
    public BookingStatus Status { get; set; }
    public string? SpecialRequests { get; set; }
}

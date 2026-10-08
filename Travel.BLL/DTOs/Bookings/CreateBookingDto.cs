using System.ComponentModel.DataAnnotations;

namespace Travel.BLL.DTOs.Bookings;

public class CreateBookingDto
{
    [Required]
    public int TravelPackageId { get; set; }

    [Required]
    public DateTime TravelDate { get; set; }

    [Range(1, 50)]
    public int NumberOfTravelers { get; set; }

    [MaxLength(1000)]
    public string? SpecialRequests { get; set; }
}

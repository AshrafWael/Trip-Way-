using System.ComponentModel.DataAnnotations;

namespace Travel.BLL.DTOs.Hotels;

public class CreateHotelBookingDto
{
    [Required]
    public int HotelId { get; set; }

    [Required]
    public DateTime CheckInDate { get; set; }

    [Required]
    public DateTime CheckOutDate { get; set; }

    [Range(1, 100)]
    public int NumberOfRooms { get; set; } = 1;

    [Range(1, 500)]
    public int NumberOfGuests { get; set; } = 1;

    [MaxLength(1000)]
    public string? SpecialRequests { get; set; }
}

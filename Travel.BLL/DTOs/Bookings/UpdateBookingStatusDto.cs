using System.ComponentModel.DataAnnotations;
using Travel.DAL.Entities;

namespace Travel.BLL.DTOs.Bookings;

public class UpdateBookingStatusDto
{
    [Required]
    public BookingStatus Status { get; set; }
}

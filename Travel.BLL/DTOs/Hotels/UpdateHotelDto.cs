using System.ComponentModel.DataAnnotations;

namespace Travel.BLL.DTOs.Hotels;

public class UpdateHotelDto : CreateHotelDto
{
    [Display(Name = "IsActive")]
    public bool IsActive { get; set; } = true;
}

using System.ComponentModel.DataAnnotations;

namespace Travel.BLL.DTOs.Destinations;

public class UpdateDestinationDto : CreateDestinationDto
{
    [Display(Name = "IsActive")]
    public bool IsActive { get; set; } = true;
}

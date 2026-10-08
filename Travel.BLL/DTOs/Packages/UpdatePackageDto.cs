using System.ComponentModel.DataAnnotations;

namespace Travel.BLL.DTOs.Packages;

public class UpdatePackageDto : CreatePackageDto
{
    [Display(Name = "IsActive")]
    public bool IsActive { get; set; } = true;
}

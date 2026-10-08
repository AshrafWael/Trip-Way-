using System.ComponentModel.DataAnnotations;

namespace Travel.BLL.DTOs.Destinations;

public class CreateDestinationDto
{
    [Required, MaxLength(150), Display(Name = "Name")]
    public string Name { get; set; } = string.Empty;

    [Required, MaxLength(150), Display(Name = "ArabicName")]
    public string ArabicName { get; set; } = string.Empty;

    [Required, MaxLength(4000), Display(Name = "Description")]
    public string Description { get; set; } = string.Empty;

    [Required, MaxLength(4000), Display(Name = "ArabicDescription")]
    public string ArabicDescription { get; set; } = string.Empty;

    [Required, MaxLength(100), Display(Name = "Country")]
    public string Country { get; set; } = string.Empty;

    [Required, MaxLength(100), Display(Name = "City")]
    public string City { get; set; } = string.Empty;

    [MaxLength(500), Display(Name = "ImageUrl")]
    public string? ImageUrl { get; set; }

    [Display(Name = "IsFeatured")]
    public bool IsFeatured { get; set; }
}

namespace Travel.BLL.DTOs.Destinations;

public class DestinationDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string ArabicName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string ArabicDescription { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public bool IsFeatured { get; set; }
    public bool IsActive { get; set; }
    public int PackageCount { get; set; }
}

using Travel.BLL.DTOs.Reviews;

namespace Travel.BLL.DTOs.Packages;

/// <summary>Full shape used on the package details page.</summary>
public class PackageDetailsDto : PackageDto
{
    public string Description { get; set; } = string.Empty;
    public string ArabicDescription { get; set; } = string.Empty;
    public int MaxTravelers { get; set; }
    public List<PackageImageDto> Images { get; set; } = new();
    public List<ReviewDto> Reviews { get; set; } = new();
}

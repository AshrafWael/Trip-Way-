namespace Travel.DAL.Entities;

public class PackageImage
{
    public int Id { get; set; }

    public int TravelPackageId { get; set; }
    public TravelPackage TravelPackage { get; set; } = null!;

    public string ImageUrl { get; set; } = string.Empty;
    public int DisplayOrder { get; set; }
}

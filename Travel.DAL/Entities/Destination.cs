namespace Travel.DAL.Entities;

public class Destination
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
    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public ICollection<TravelPackage> TravelPackages { get; set; } = new List<TravelPackage>();
    public ICollection<Hotel> Hotels { get; set; } = new List<Hotel>();
}

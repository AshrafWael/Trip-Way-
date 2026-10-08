namespace Travel.DAL.Entities;

public class Review
{
    public int Id { get; set; }

    public string UserId { get; set; } = string.Empty;
    public ApplicationUser User { get; set; } = null!;

    public int TravelPackageId { get; set; }
    public TravelPackage TravelPackage { get; set; } = null!;

    public int Rating { get; set; }
    public string? Comment { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool IsApproved { get; set; }
}

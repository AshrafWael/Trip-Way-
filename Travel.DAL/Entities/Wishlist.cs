namespace Travel.DAL.Entities;

public class Wishlist
{
    public int Id { get; set; }

    public string UserId { get; set; } = string.Empty;
    public ApplicationUser User { get; set; } = null!;

    public int TravelPackageId { get; set; }
    public TravelPackage TravelPackage { get; set; } = null!;

    public DateTime AddedAt { get; set; } = DateTime.UtcNow;
}

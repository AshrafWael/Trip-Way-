namespace Travel.BLL.DTOs.Wishlist;

public class WishlistItemDto
{
    public int TravelPackageId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? MainImageUrl { get; set; }
    public decimal? Price { get; set; }
    public decimal? DiscountPrice { get; set; }
    public DateTime AddedAt { get; set; }
}

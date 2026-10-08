using Microsoft.AspNetCore.Identity;

namespace Travel.DAL.Entities;

/// <summary>
/// Extends ASP.NET Core Identity's user with the profile fields the
/// travel platform needs. Roles ("Admin", "Customer") are assigned
/// through Identity's role system, not stored as a column here.
/// </summary>
public class ApplicationUser : IdentityUser
{
    public string FullName { get; set; } = string.Empty;

    public string? ProfileImage { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    public ICollection<HotelBooking> HotelBookings { get; set; } = new List<HotelBooking>();
    public ICollection<Wishlist> WishlistItems { get; set; } = new List<Wishlist>();
    public ICollection<Review> Reviews { get; set; } = new List<Review>();
    public ICollection<BlogPost> BlogPosts { get; set; } = new List<BlogPost>();
}

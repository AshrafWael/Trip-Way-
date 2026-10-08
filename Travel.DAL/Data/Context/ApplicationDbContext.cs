using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Travel.DAL.Entities;

namespace Travel.DAL.Data.Context;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Destination> Destinations => Set<Destination>();
    public DbSet<TravelPackage> TravelPackages => Set<TravelPackage>();
    public DbSet<PackageImage> PackageImages => Set<PackageImage>();
    public DbSet<Hotel> Hotels => Set<Hotel>();
    public DbSet<HotelImage> HotelImages => Set<HotelImage>();
    public DbSet<HotelBooking> HotelBookings => Set<HotelBooking>();
    public DbSet<Booking> Bookings => Set<Booking>();
    public DbSet<Wishlist> Wishlists => Set<Wishlist>();
    public DbSet<Review> Reviews => Set<Review>();
    public DbSet<BlogPost> BlogPosts => Set<BlogPost>();
    public DbSet<BlogPostImage> BlogPostImages => Set<BlogPostImage>();
    public DbSet<SiteSettings> SiteSettings => Set<SiteSettings>();
    public DbSet<ContactMessage> ContactMessages => Set<ContactMessage>();
    public DbSet<NewsletterSubscriber> NewsletterSubscribers => Set<NewsletterSubscriber>();
    //public DbSet<NewsletterSubscriber> NewsletterSubscribers => Set<NewsletterSubscriber>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Every IEntityTypeConfiguration<T> in this assembly is picked up
        // automatically — no configuration is registered by hand here,
        // so adding a new entity config is a one-file change.
        builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}

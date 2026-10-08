using Travel.DAL.Entities;
using Travel.DAL.Repositories;

namespace Travel.DAL.UnitOfWork;

/// <summary>
/// Groups repositories that must commit together behind a single
/// SaveChangesAsync, so a service method that touches more than one
/// entity (e.g. Booking + decrementing AvailableSeats) does so
/// atomically.
/// </summary>
public interface IUnitOfWork : IDisposable
{
    IGenericRepository<Destination> Destinations { get; }
    IPackageRepository Packages { get; }
    IGenericRepository<PackageImage> PackageImages { get; }
    IGenericRepository<Hotel> Hotels { get; }
    IGenericRepository<HotelImage> HotelImages { get; }
    IGenericRepository<HotelBooking> HotelBookings { get; }
    IGenericRepository<BlogPostImage> BlogPostImages { get; }
    IGenericRepository<SiteSettings> SiteSettings { get; }
    IGenericRepository<Booking> Bookings { get; }
    IGenericRepository<Wishlist> Wishlists { get; }
    IGenericRepository<Review> Reviews { get; }
    IGenericRepository<BlogPost> BlogPosts { get; }
    IGenericRepository<ContactMessage> ContactMessages { get; }
    IGenericRepository<NewsletterSubscriber> NewsletterSubscribers { get; }
    //IGenericRepository<NewsletterSubscriber> NewsletterSubscribers { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}

using Travel.DAL.Data.Context;
using Travel.DAL.Entities;
using Travel.DAL.Repositories;

namespace Travel.DAL.UnitOfWork;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;

    private IGenericRepository<Destination>? _destinations;
    private IPackageRepository? _packages;
    private IGenericRepository<PackageImage>? _packageImages;
    private IGenericRepository<Hotel>? _hotels;
    private IGenericRepository<HotelImage>? _hotelImages;
    private IGenericRepository<HotelBooking>? _hotelBookings;
    private IGenericRepository<BlogPostImage>? _blogPostImages;
    private IGenericRepository<SiteSettings>? _siteSettings;
    private IGenericRepository<Booking>? _bookings;
    private IGenericRepository<Wishlist>? _wishlists;
    private IGenericRepository<Review>? _reviews;
    private IGenericRepository<BlogPost>? _blogPosts;
    private IGenericRepository<ContactMessage>? _contactMessages;
    private IGenericRepository<NewsletterSubscriber>? _newsletterSubscribers;
    //private IGenericRepository<NewsletterSubscriber>? _newsletterSubscribers;

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
    }

    public IGenericRepository<Destination> Destinations
        => _destinations ??= new GenericRepository<Destination>(_context);

    public IPackageRepository Packages
        => _packages ??= new PackageRepository(_context);

    public IGenericRepository<PackageImage> PackageImages
        => _packageImages ??= new GenericRepository<PackageImage>(_context);

    public IGenericRepository<Hotel> Hotels
        => _hotels ??= new GenericRepository<Hotel>(_context);

    public IGenericRepository<HotelImage> HotelImages
        => _hotelImages ??= new GenericRepository<HotelImage>(_context);

    public IGenericRepository<HotelBooking> HotelBookings
        => _hotelBookings ??= new GenericRepository<HotelBooking>(_context);

    public IGenericRepository<BlogPostImage> BlogPostImages
        => _blogPostImages ??= new GenericRepository<BlogPostImage>(_context);

    public IGenericRepository<SiteSettings> SiteSettings
        => _siteSettings ??= new GenericRepository<SiteSettings>(_context);

    public IGenericRepository<Booking> Bookings
        => _bookings ??= new GenericRepository<Booking>(_context);

    public IGenericRepository<Wishlist> Wishlists
        => _wishlists ??= new GenericRepository<Wishlist>(_context);

    public IGenericRepository<Review> Reviews
        => _reviews ??= new GenericRepository<Review>(_context);

    public IGenericRepository<BlogPost> BlogPosts
        => _blogPosts ??= new GenericRepository<BlogPost>(_context);

    public IGenericRepository<ContactMessage> ContactMessages
        => _contactMessages ??= new GenericRepository<ContactMessage>(_context);

    public IGenericRepository<NewsletterSubscriber> NewsletterSubscribers
        => _newsletterSubscribers ??= new GenericRepository<NewsletterSubscriber>(_context);

    //public IGenericRepository<NewsletterSubscriber> NewsletterSubscribers
    //    => _newsletterSubscribers ??= new GenericRepository<NewsletterSubscriber>(_context);

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => await _context.SaveChangesAsync(cancellationToken);

    public void Dispose()
    {
        _context.Dispose();
        GC.SuppressFinalize(this);
    }
}

using AutoMapper;
using Travel.BLL.DTOs.Blog;
using Travel.BLL.DTOs.Bookings;
using Travel.BLL.DTOs.Contact;
using Travel.BLL.DTOs.Destinations;
using Travel.BLL.DTOs.Hotels;
using Travel.BLL.DTOs.Packages;
using Travel.BLL.DTOs.Reviews;
using Travel.BLL.DTOs.Settings;
using Travel.BLL.DTOs.Wishlist;
using Travel.DAL.Entities;

namespace Travel.BLL.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Destinations
        CreateMap<Destination, DestinationDto>()
            .ForMember(d => d.PackageCount, opt => opt.MapFrom(s => s.TravelPackages.Count));
        CreateMap<CreateDestinationDto, Destination>();
        CreateMap<UpdateDestinationDto, Destination>();

        // Packages
        CreateMap<TravelPackage, PackageDto>()
            .ForMember(d => d.DestinationName, opt => opt.MapFrom(s => s.Destination != null ? s.Destination.Name : string.Empty))
            .ForMember(d => d.AverageRating, opt => opt.MapFrom(s =>
                s.Reviews.Any(r => r.IsApproved) ? s.Reviews.Where(r => r.IsApproved).Average(r => r.Rating) : 0))
            .ForMember(d => d.ReviewCount, opt => opt.MapFrom(s => s.Reviews.Count(r => r.IsApproved)));

        CreateMap<TravelPackage, PackageDetailsDto>()
            .IncludeBase<TravelPackage, PackageDto>()
            .ForMember(d => d.Images, opt => opt.MapFrom(s => s.Images.OrderBy(i => i.DisplayOrder)))
            .ForMember(d => d.Reviews, opt => opt.MapFrom(s => s.Reviews.Where(r => r.IsApproved)));

        CreateMap<PackageImage, PackageImageDto>();

        CreateMap<CreatePackageDto, TravelPackage>()
            .ForMember(d => d.Images, opt => opt.Ignore());
        CreateMap<UpdatePackageDto, TravelPackage>()
            .ForMember(d => d.Images, opt => opt.Ignore());

        // Bookings
        CreateMap<Booking, BookingDto>()
            .ForMember(d => d.PackageTitle, opt => opt.MapFrom(s => s.TravelPackage.Title))
            .ForMember(d => d.PackageImageUrl, opt => opt.MapFrom(s => s.TravelPackage.MainImageUrl));

        // Hotels
        CreateMap<Hotel, HotelDto>()
            .ForMember(d => d.DestinationName, opt => opt.MapFrom(s => s.Destination != null ? s.Destination.Name : string.Empty));

        CreateMap<Hotel, HotelDetailsDto>()
            .IncludeBase<Hotel, HotelDto>()
            .ForMember(d => d.Images, opt => opt.MapFrom(s => s.Images.OrderBy(i => i.DisplayOrder)));

        CreateMap<HotelImage, HotelImageDto>();

        CreateMap<CreateHotelDto, Hotel>()
            .ForMember(d => d.Images, opt => opt.Ignore());
        CreateMap<UpdateHotelDto, Hotel>()
            .ForMember(d => d.Images, opt => opt.Ignore());

        // Hotel bookings
        CreateMap<HotelBooking, HotelBookingDto>()
            .ForMember(d => d.HotelName, opt => opt.MapFrom(s => s.Hotel.Name))
            .ForMember(d => d.HotelImageUrl, opt => opt.MapFrom(s => s.Hotel.MainImageUrl));

        // Wishlist
        CreateMap<Wishlist, WishlistItemDto>()
            .ForMember(d => d.TravelPackageId, opt => opt.MapFrom(s => s.TravelPackageId))
            .ForMember(d => d.Title, opt => opt.MapFrom(s => s.TravelPackage.Title))
            .ForMember(d => d.MainImageUrl, opt => opt.MapFrom(s => s.TravelPackage.MainImageUrl))
            .ForMember(d => d.Price, opt => opt.MapFrom(s => s.TravelPackage.Price))
            .ForMember(d => d.DiscountPrice, opt => opt.MapFrom(s => s.TravelPackage.DiscountPrice));

        // Reviews
        CreateMap<Review, ReviewDto>()
            .ForMember(d => d.PackageTitle, opt => opt.MapFrom(s => s.TravelPackage != null ? s.TravelPackage.Title : string.Empty))
            .ForMember(d => d.UserFullName, opt => opt.MapFrom(s => s.User != null ? s.User.FullName : string.Empty));
        CreateMap<CreateReviewDto, Review>();

        // Blog
        CreateMap<BlogPost, BlogPostDto>()
            .ForMember(d => d.AuthorName, opt => opt.MapFrom(s => s.Author.FullName))
            .ForMember(d => d.Images, opt => opt.MapFrom(s => s.Images.OrderBy(i => i.DisplayOrder)));
        CreateMap<BlogPostImage, BlogPostImageDto>();
        CreateMap<CreateBlogPostDto, BlogPost>()
            .ForMember(d => d.Images, opt => opt.Ignore());
        CreateMap<UpdateBlogPostDto, BlogPost>()
            .ForMember(d => d.Images, opt => opt.Ignore());

        // Contact
        CreateMap<ContactMessage, ContactMessageDto>();
        CreateMap<CreateContactMessageDto, ContactMessage>();

        // Site settings
        CreateMap<SiteSettings, SiteSettingsDto>();
        CreateMap<UpdateSiteSettingsDto, SiteSettings>();
    }
}

using Travel.BLL.DTOs.Admin;
using Travel.BLL.DTOs.Auth;
using Travel.BLL.DTOs.Blog;
using Travel.BLL.DTOs.Bookings;
using Travel.BLL.DTOs.Common;
using Travel.BLL.DTOs.Contact;
using Travel.BLL.DTOs.Destinations;
using Travel.BLL.DTOs.Hotels;
using Travel.BLL.DTOs.Packages;
using Travel.BLL.DTOs.Reviews;
using Travel.BLL.DTOs.Settings;
using Travel.BLL.DTOs.Wishlist;
using Travel.DAL.Entities;

namespace Travel.Web.Services.ApiClient;

/// <summary>
/// Everything Travel.Web needs from Travel.API, expressed as plain
/// methods so controllers never build an HttpRequestMessage by hand.
/// </summary>
public interface ITravelApiClient
{
    Task<AuthResponseDto> LoginAsync(LoginDto dto);
    Task<AuthResponseDto> RegisterAsync(RegisterDto dto);

    Task<IReadOnlyList<DestinationDto>> GetDestinationsAsync();
    Task<DestinationDto> GetDestinationAsync(int id);

    Task<PagedResultDto<PackageDto>> SearchPackagesAsync(PackageSearchParamsDto searchParams);
    Task<PackageDetailsDto> GetPackageAsync(int id);

    Task<PagedResultDto<HotelDto>> SearchHotelsAsync(HotelSearchParamsDto searchParams);
    Task<HotelDetailsDto> GetHotelAsync(int id);

    Task<IReadOnlyList<BlogPostDto>> GetBlogPostsAsync();
    Task<BlogPostDto> GetBlogPostAsync(int id);

    Task SubmitContactMessageAsync(CreateContactMessageDto dto);
    Task<string> SubscribeToNewsletterAsync(string email);
    //Task SubscribeToNewsletterAsync(string email);

    // ---- Customer account (bookings/wishlist/reviews) ----

    Task<IReadOnlyList<BookingDto>> GetMyBookingsAsync();
    Task<BookingDto> CreateBookingAsync(CreateBookingDto dto);
    Task CancelBookingAsync(int id);

    Task<IReadOnlyList<HotelBookingDto>> GetMyHotelBookingsAsync();
    Task<HotelBookingDto> CreateHotelBookingAsync(CreateHotelBookingDto dto);
    Task CancelHotelBookingAsync(int id);

    Task<IReadOnlyList<WishlistItemDto>> GetMyWishlistAsync();
    Task AddToWishlistAsync(int packageId);
    Task RemoveFromWishlistAsync(int packageId);

    Task<ReviewDto> SubmitReviewAsync(int packageId, CreateReviewDto dto);

    // ---- Admin ----

    Task<DashboardStatsDto> GetDashboardStatsAsync();

    Task<IReadOnlyList<DestinationDto>> GetDestinationsAsync(bool includeInactive);
    Task<DestinationDto> CreateDestinationAsync(CreateDestinationDto dto);
    Task UpdateDestinationAsync(int id, UpdateDestinationDto dto);
    Task DeleteDestinationAsync(int id);

    Task<IReadOnlyList<PackageDto>> GetAllPackagesForAdminAsync();
    Task<PackageDto> CreatePackageAsync(CreatePackageDto dto);
    Task UpdatePackageAsync(int id, UpdatePackageDto dto);
    Task UpdatePackageImagesAsync(int id, List<string> imageUrls);
    Task DeletePackageAsync(int id);

    Task<IReadOnlyList<HotelDto>> GetAllHotelsForAdminAsync();
    Task<HotelDto> CreateHotelAsync(CreateHotelDto dto);
    Task UpdateHotelAsync(int id, UpdateHotelDto dto);
    Task UpdateHotelImagesAsync(int id, List<string> imageUrls);
    Task DeleteHotelAsync(int id);

    Task<IReadOnlyList<BookingDto>> GetAllBookingsForAdminAsync();
    Task UpdateBookingStatusAsync(int id, BookingStatus status);

    Task<IReadOnlyList<HotelBookingDto>> GetAllHotelBookingsForAdminAsync();
    Task UpdateHotelBookingStatusAsync(int id, BookingStatus status);

    Task<IReadOnlyList<ReviewDto>> GetAllReviewsForAdminAsync();
    Task ApproveReviewAsync(int id);
    Task DeleteReviewAsync(int id);

    Task<IReadOnlyList<BlogPostDto>> GetAllBlogPostsForAdminAsync();
    Task<BlogPostDto> CreateBlogPostAsync(CreateBlogPostDto dto);
    Task UpdateBlogPostAsync(int id, UpdateBlogPostDto dto);
    Task UpdateBlogPostImagesAsync(int id, List<string> imageUrls);

    Task<SiteSettingsDto> GetSiteSettingsAsync();
    Task UpdateSiteSettingsAsync(UpdateSiteSettingsDto dto);
    Task DeleteBlogPostAsync(int id);

    Task<IReadOnlyList<ContactMessageDto>> GetContactMessagesAsync();
    Task MarkContactMessageReadAsync(int id);

    Task<IReadOnlyList<UserSummaryDto>> GetUsersAsync();
    Task LockUserAsync(string id);
    Task UnlockUserAsync(string id);
}

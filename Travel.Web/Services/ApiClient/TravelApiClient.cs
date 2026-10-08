using System.Text.Json;
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

public class TravelApiClient : ITravelApiClient
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly HttpClient _httpClient;

    public TravelApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public Task<AuthResponseDto> LoginAsync(LoginDto dto) =>
        PostAsync<LoginDto, AuthResponseDto>("api/auth/login", dto);

    public Task<AuthResponseDto> RegisterAsync(RegisterDto dto) =>
        PostAsync<RegisterDto, AuthResponseDto>("api/auth/register", dto);

    public Task<IReadOnlyList<DestinationDto>> GetDestinationsAsync() =>
        GetAsync<IReadOnlyList<DestinationDto>>("api/destinations");

    public Task<DestinationDto> GetDestinationAsync(int id) =>
        GetAsync<DestinationDto>($"api/destinations/{id}");

    public Task<PagedResultDto<PackageDto>> SearchPackagesAsync(PackageSearchParamsDto searchParams)
    {
        var query = BuildQueryString(searchParams);
        return GetAsync<PagedResultDto<PackageDto>>($"api/packages{query}");
    }

    public Task<PackageDetailsDto> GetPackageAsync(int id) =>
        GetAsync<PackageDetailsDto>($"api/packages/{id}");

    public Task<PagedResultDto<HotelDto>> SearchHotelsAsync(HotelSearchParamsDto searchParams)
    {
        var query = BuildQueryString(searchParams);
        return GetAsync<PagedResultDto<HotelDto>>($"api/hotels{query}");
    }

    public Task<HotelDetailsDto> GetHotelAsync(int id) =>
        GetAsync<HotelDetailsDto>($"api/hotels/{id}");

    public Task<IReadOnlyList<BlogPostDto>> GetBlogPostsAsync() =>
        GetAsync<IReadOnlyList<BlogPostDto>>("api/blog");

    public Task<BlogPostDto> GetBlogPostAsync(int id) =>
        GetAsync<BlogPostDto>($"api/blog/{id}");

    public async Task SubmitContactMessageAsync(CreateContactMessageDto dto) =>
        await PostAsync<CreateContactMessageDto, ContactMessageDto>("api/contact", dto);

    public Task<string> SubscribeToNewsletterAsync(string email) =>
        PostAndGetMessageAsync("api/newsletter/subscribe", new { Email = email });

    //public async Task SubscribeToNewsletterAsync(string email) =>
    //    await PostAsync<object, object>("api/newsletter/subscribe", new { Email = email });

    // ---- Customer account (bookings/wishlist/reviews) ----

    public Task<IReadOnlyList<BookingDto>> GetMyBookingsAsync() =>
        GetAsync<IReadOnlyList<BookingDto>>("api/bookings/my");

    public Task<BookingDto> CreateBookingAsync(CreateBookingDto dto) =>
        PostAsync<CreateBookingDto, BookingDto>("api/bookings", dto);

    public Task CancelBookingAsync(int id) =>
        PutAsync($"api/bookings/{id}/cancel", new { });

    public Task<IReadOnlyList<HotelBookingDto>> GetMyHotelBookingsAsync() =>
        GetAsync<IReadOnlyList<HotelBookingDto>>("api/hotel-bookings/my");

    public Task<HotelBookingDto> CreateHotelBookingAsync(CreateHotelBookingDto dto) =>
        PostAsync<CreateHotelBookingDto, HotelBookingDto>("api/hotel-bookings", dto);

    public Task CancelHotelBookingAsync(int id) =>
        PutAsync($"api/hotel-bookings/{id}/cancel", new { });

    public Task<IReadOnlyList<WishlistItemDto>> GetMyWishlistAsync() =>
        GetAsync<IReadOnlyList<WishlistItemDto>>("api/wishlist");

    public Task AddToWishlistAsync(int packageId) =>
        PostNoBodyAsync($"api/wishlist/{packageId}");

    public Task RemoveFromWishlistAsync(int packageId) =>
        DeleteAsync($"api/wishlist/{packageId}");

    public Task<ReviewDto> SubmitReviewAsync(int packageId, CreateReviewDto dto) =>
        PostAsync<CreateReviewDto, ReviewDto>($"api/packages/{packageId}/reviews", dto);

    // ---- Admin ----

    public Task<DashboardStatsDto> GetDashboardStatsAsync() =>
        GetAsync<DashboardStatsDto>("api/admin/dashboard");

    public Task<IReadOnlyList<DestinationDto>> GetDestinationsAsync(bool includeInactive) =>
        GetAsync<IReadOnlyList<DestinationDto>>($"api/destinations?includeInactive={includeInactive}");

    public Task<DestinationDto> CreateDestinationAsync(CreateDestinationDto dto) =>
        PostAsync<CreateDestinationDto, DestinationDto>("api/destinations", dto);

    public Task UpdateDestinationAsync(int id, UpdateDestinationDto dto) =>
        PutAsync($"api/destinations/{id}", dto);

    public Task DeleteDestinationAsync(int id) =>
        DeleteAsync($"api/destinations/{id}");

    public Task<IReadOnlyList<PackageDto>> GetAllPackagesForAdminAsync() =>
        GetAsync<IReadOnlyList<PackageDto>>("api/packages/admin/all");

    public Task<PackageDto> CreatePackageAsync(CreatePackageDto dto) =>
        PostAsync<CreatePackageDto, PackageDto>("api/packages", dto);

    public Task UpdatePackageAsync(int id, UpdatePackageDto dto) =>
        PutAsync($"api/packages/{id}", dto);

    public Task UpdatePackageImagesAsync(int id, List<string> imageUrls) =>
        PutAsync($"api/packages/{id}/images", imageUrls);

    public Task DeletePackageAsync(int id) =>
        DeleteAsync($"api/packages/{id}");

    public Task<IReadOnlyList<HotelDto>> GetAllHotelsForAdminAsync() =>
        GetAsync<IReadOnlyList<HotelDto>>("api/hotels/admin/all");

    public Task<HotelDto> CreateHotelAsync(CreateHotelDto dto) =>
        PostAsync<CreateHotelDto, HotelDto>("api/hotels", dto);

    public Task UpdateHotelAsync(int id, UpdateHotelDto dto) =>
        PutAsync($"api/hotels/{id}", dto);

    public Task UpdateHotelImagesAsync(int id, List<string> imageUrls) =>
        PutAsync($"api/hotels/{id}/images", imageUrls);

    public Task DeleteHotelAsync(int id) =>
        DeleteAsync($"api/hotels/{id}");

    public Task<IReadOnlyList<BookingDto>> GetAllBookingsForAdminAsync() =>
        GetAsync<IReadOnlyList<BookingDto>>("api/admin/bookings");

    public Task UpdateBookingStatusAsync(int id, BookingStatus status) =>
        PutAsync($"api/admin/bookings/{id}/status", new UpdateBookingStatusDto { Status = status });

    public Task<IReadOnlyList<HotelBookingDto>> GetAllHotelBookingsForAdminAsync() =>
        GetAsync<IReadOnlyList<HotelBookingDto>>("api/admin/hotel-bookings");

    public Task UpdateHotelBookingStatusAsync(int id, BookingStatus status) =>
        PutAsync($"api/admin/hotel-bookings/{id}/status", new UpdateBookingStatusDto { Status = status });

    public Task<IReadOnlyList<ReviewDto>> GetAllReviewsForAdminAsync() =>
        GetAsync<IReadOnlyList<ReviewDto>>("api/admin/reviews");

    public Task ApproveReviewAsync(int id) =>
        PutAsync($"api/admin/reviews/{id}/approve", new { });

    public Task DeleteReviewAsync(int id) =>
        DeleteAsync($"api/admin/reviews/{id}");

    public Task<IReadOnlyList<BlogPostDto>> GetAllBlogPostsForAdminAsync() =>
        GetAsync<IReadOnlyList<BlogPostDto>>("api/blog/admin/all");

    public Task<BlogPostDto> CreateBlogPostAsync(CreateBlogPostDto dto) =>
        PostAsync<CreateBlogPostDto, BlogPostDto>("api/blog", dto);

    public Task UpdateBlogPostAsync(int id, UpdateBlogPostDto dto) =>
        PutAsync($"api/blog/{id}", dto);

    public Task UpdateBlogPostImagesAsync(int id, List<string> imageUrls) =>
        PutAsync($"api/blog/{id}/images", imageUrls);

    public Task<SiteSettingsDto> GetSiteSettingsAsync() =>
        GetAsync<SiteSettingsDto>("api/settings");

    public Task UpdateSiteSettingsAsync(UpdateSiteSettingsDto dto) =>
        PutAsync("api/settings", dto);

    public Task DeleteBlogPostAsync(int id) =>
        DeleteAsync($"api/blog/{id}");

    public Task<IReadOnlyList<ContactMessageDto>> GetContactMessagesAsync() =>
        GetAsync<IReadOnlyList<ContactMessageDto>>("api/contact");

    public Task MarkContactMessageReadAsync(int id) =>
        PutAsync($"api/contact/{id}/read", new { });

    public Task<IReadOnlyList<UserSummaryDto>> GetUsersAsync() =>
        GetAsync<IReadOnlyList<UserSummaryDto>>("api/admin/users");

    public Task LockUserAsync(string id) =>
        PutAsync($"api/admin/users/{id}/lock", new { });

    public Task UnlockUserAsync(string id) =>
        PutAsync($"api/admin/users/{id}/unlock", new { });

    private static string BuildQueryString(PackageSearchParamsDto p)
    {
        var parts = new List<string>
        {
            $"pageNumber={p.PageNumber}",
            $"pageSize={p.PageSize}"
        };

        if (p.DestinationId.HasValue) parts.Add($"destinationId={p.DestinationId}");
        if (p.MinPrice.HasValue) parts.Add($"minPrice={p.MinPrice}");
        if (p.MaxPrice.HasValue) parts.Add($"maxPrice={p.MaxPrice}");
        if (p.Travelers.HasValue) parts.Add($"travelers={p.Travelers}");
        if (!string.IsNullOrWhiteSpace(p.Search)) parts.Add($"search={Uri.EscapeDataString(p.Search)}");
        if (!string.IsNullOrWhiteSpace(p.SortBy)) parts.Add($"sortBy={Uri.EscapeDataString(p.SortBy)}");

        return "?" + string.Join("&", parts);
    }

    private static string BuildQueryString(HotelSearchParamsDto p)
    {
        var parts = new List<string>
        {
            $"pageNumber={p.PageNumber}",
            $"pageSize={p.PageSize}"
        };

        if (p.DestinationId.HasValue) parts.Add($"destinationId={p.DestinationId}");
        if (p.MinPrice.HasValue) parts.Add($"minPrice={p.MinPrice}");
        if (p.MaxPrice.HasValue) parts.Add($"maxPrice={p.MaxPrice}");
        if (p.MinStarRating.HasValue) parts.Add($"minStarRating={p.MinStarRating}");
        if (!string.IsNullOrWhiteSpace(p.Search)) parts.Add($"search={Uri.EscapeDataString(p.Search)}");
        if (!string.IsNullOrWhiteSpace(p.SortBy)) parts.Add($"sortBy={Uri.EscapeDataString(p.SortBy)}");

        return "?" + string.Join("&", parts);
    }

    private async Task<T> GetAsync<T>(string path)
    {
        var response = await _httpClient.GetAsync(path);
        return await UnwrapAsync<T>(response);
    }

    private async Task<TResponse> PostAsync<TRequest, TResponse>(string path, TRequest body)
    {
        var response = await _httpClient.PostAsJsonAsync(path, body);
        return await UnwrapAsync<TResponse>(response);
    }

    private async Task PutAsync<TRequest>(string path, TRequest body)
    {
        var response = await _httpClient.PutAsJsonAsync(path, body);
        await EnsureSuccessAsync(response);
    }

    private async Task PostNoBodyAsync(string path)
    {
        var response = await _httpClient.PostAsync(path, content: null);
        await EnsureSuccessAsync(response);
    }

    private async Task<string> PostAndGetMessageAsync<TRequest>(string path, TRequest body)
    {
        var response = await _httpClient.PostAsJsonAsync(path, body);
        await EnsureSuccessAsync(response);

        var raw = await response.Content.ReadAsStringAsync();
        var envelope = JsonSerializer.Deserialize<ApiResponse<object>>(raw, JsonOptions);
        return envelope?.Message ?? string.Empty;
    }

    private async Task DeleteAsync(string path)
    {
        var response = await _httpClient.DeleteAsync(path);
        await EnsureSuccessAsync(response);
    }

    private static async Task EnsureSuccessAsync(HttpResponseMessage response)
    {
        if (response.IsSuccessStatusCode)
            return;

        var raw = await response.Content.ReadAsStringAsync();
        var envelope = string.IsNullOrWhiteSpace(raw)
            ? null
            : JsonSerializer.Deserialize<ApiResponse<object>>(raw, JsonOptions);

        var message = envelope?.Message ?? $"Request failed with status {(int)response.StatusCode}.";
        throw new ApiException(response.StatusCode, message, envelope?.Errors);
    }

    private static async Task<T> UnwrapAsync<T>(HttpResponseMessage response)
    {
        var raw = await response.Content.ReadAsStringAsync();
        var envelope = string.IsNullOrWhiteSpace(raw)
            ? null
            : JsonSerializer.Deserialize<ApiResponse<T>>(raw, JsonOptions);

        if (!response.IsSuccessStatusCode || envelope is null || !envelope.Success)
        {
            var message = envelope?.Message ?? $"Request failed with status {(int)response.StatusCode}.";
            throw new ApiException(response.StatusCode, message, envelope?.Errors);
        }

        return envelope.Data!;
    }
}

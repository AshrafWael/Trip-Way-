using Microsoft.AspNetCore.Mvc;
using Travel.BLL.DTOs.Hotels;
using Travel.BLL.DTOs.Packages;
using Travel.BLL.DTOs.Settings;
using Travel.Web.Services.ApiClient;
using Travel.Web.ViewModels.Home;

namespace Travel.Web.Controllers;

public class HomeController : Controller
{
    private readonly ITravelApiClient _apiClient;
    private readonly ILogger<HomeController> _logger;

    public HomeController(ITravelApiClient apiClient, ILogger<HomeController> logger)
    {
        _apiClient = apiClient;
        _logger = logger;
    }

    public async Task<IActionResult> Index()
    {
        try
        {
            var destinations = await _apiClient.GetDestinationsAsync();
            var packages = await _apiClient.SearchPackagesAsync(new PackageSearchParamsDto
            {
                PageNumber = 1,
                PageSize = 6,
                SortBy = "featured"
            });

            var hotels = await _apiClient.SearchHotelsAsync(new HotelSearchParamsDto
            {
                PageNumber = 1,
                PageSize = 6,
                SortBy = "featured"
            });

            SiteSettingsDto? settings = null;
            try
            {
                settings = await _apiClient.GetSiteSettingsAsync();
            }
            catch (ApiException ex)
            {
                // The hero still renders from the localized defaults
                // below if settings can't be loaded for any reason.
                _logger.LogWarning(ex, "Failed to load site settings from the API.");
            }

            return View(new HomeViewModel
            {
                FeaturedDestinations = destinations.Where(d => d.IsFeatured).Take(6).ToList(),
                FeaturedPackages = packages.Items,
                FeaturedHotels = hotels.Items,
                Settings = settings
            });
        }
        catch (ApiException ex)
        {
            // The homepage should still render (empty state) rather than
            // 500 if the API is temporarily unreachable during local dev.
            _logger.LogWarning(ex, "Failed to load homepage data from the API.");
            return View(new HomeViewModel());
        }
    }

    public IActionResult Privacy() => View();

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error() => View();
}

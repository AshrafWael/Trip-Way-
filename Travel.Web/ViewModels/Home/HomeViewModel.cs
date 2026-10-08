using Travel.BLL.DTOs.Destinations;
using Travel.BLL.DTOs.Hotels;
using Travel.BLL.DTOs.Packages;
using Travel.BLL.DTOs.Settings;

namespace Travel.Web.ViewModels.Home;

public class HomeViewModel
{
    public IReadOnlyList<DestinationDto> FeaturedDestinations { get; set; } = Array.Empty<DestinationDto>();
    public IReadOnlyList<PackageDto> FeaturedPackages { get; set; } = Array.Empty<PackageDto>();
    public IReadOnlyList<HotelDto> FeaturedHotels { get; set; } = Array.Empty<HotelDto>();
    public SiteSettingsDto? Settings { get; set; }
}

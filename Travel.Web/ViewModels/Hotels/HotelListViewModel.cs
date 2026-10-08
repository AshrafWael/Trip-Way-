using Travel.BLL.DTOs.Common;
using Travel.BLL.DTOs.Destinations;
using Travel.BLL.DTOs.Hotels;

namespace Travel.Web.ViewModels.Hotels;

public class HotelListViewModel
{
    public PagedResultDto<HotelDto> Result { get; set; } = new();
    public IReadOnlyList<DestinationDto> Destinations { get; set; } = Array.Empty<DestinationDto>();
    public HotelSearchParamsDto SearchParams { get; set; } = new();
}

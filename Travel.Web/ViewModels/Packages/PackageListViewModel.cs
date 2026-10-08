using Travel.BLL.DTOs.Common;
using Travel.BLL.DTOs.Destinations;
using Travel.BLL.DTOs.Packages;

namespace Travel.Web.ViewModels.Packages;

public class PackageListViewModel
{
    public PagedResultDto<PackageDto> Result { get; set; } = new();
    public IReadOnlyList<DestinationDto> Destinations { get; set; } = Array.Empty<DestinationDto>();
    public PackageSearchParamsDto SearchParams { get; set; } = new();
}

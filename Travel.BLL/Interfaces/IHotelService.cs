using Travel.BLL.DTOs.Common;
using Travel.BLL.DTOs.Hotels;

namespace Travel.BLL.Interfaces;

public interface IHotelService
{
    Task<PagedResultDto<HotelDto>> SearchAsync(HotelSearchParamsDto searchParams, CancellationToken cancellationToken = default);
    Task<HotelDetailsDto> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<HotelDto> CreateAsync(CreateHotelDto dto, CancellationToken cancellationToken = default);
    Task UpdateAsync(int id, UpdateHotelDto dto, CancellationToken cancellationToken = default);
    Task UpdateImagesAsync(int id, IReadOnlyList<string> imageUrls, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<HotelDto>> GetAllForAdminAsync(CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}

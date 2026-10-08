using Travel.BLL.DTOs.Destinations;

namespace Travel.BLL.Interfaces;

public interface IDestinationService
{
    Task<IReadOnlyList<DestinationDto>> GetAllAsync(bool onlyActive = true, CancellationToken cancellationToken = default);
    Task<DestinationDto> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<DestinationDto> CreateAsync(CreateDestinationDto dto, CancellationToken cancellationToken = default);
    Task UpdateAsync(int id, UpdateDestinationDto dto, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}

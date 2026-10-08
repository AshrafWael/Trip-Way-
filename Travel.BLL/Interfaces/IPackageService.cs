using Travel.BLL.DTOs.Common;
using Travel.BLL.DTOs.Packages;

namespace Travel.BLL.Interfaces;

public interface IPackageService
{
    Task<PagedResultDto<PackageDto>> SearchAsync(PackageSearchParamsDto searchParams, CancellationToken cancellationToken = default);
    Task<PackageDetailsDto> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<PackageDto> CreateAsync(CreatePackageDto dto, CancellationToken cancellationToken = default);
    Task UpdateAsync(int id, UpdatePackageDto dto, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>Admin package management list -- includes inactive packages, unlike SearchAsync.</summary>
    Task<IReadOnlyList<PackageDto>> GetAllForAdminAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Replaces the package's entire image gallery with the given URLs, in
    /// order. Simpler and less error-prone for an admin form than
    /// per-image add/remove endpoints -- the whole gallery is small and
    /// edited as a unit.
    /// </summary>
    Task UpdateImagesAsync(int id, IReadOnlyList<string> imageUrls, CancellationToken cancellationToken = default);
}

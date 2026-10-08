using Travel.DAL.Entities;

namespace Travel.DAL.Repositories;

/// <summary>
/// TravelPackage listing/search always needs Destination + Images eagerly
/// loaded plus multi-field filtering, sorting and paging. Expressing that
/// through the generic repository's Query() would just push the same
/// Include/Where chain into every service method that touches packages,
/// so it's centralized here instead.
/// </summary>
public interface IPackageRepository : IGenericRepository<TravelPackage>
{
    Task<(IReadOnlyList<TravelPackage> Items, int TotalCount)> SearchAsync(
        int pageNumber,
        int pageSize,
        int? destinationId = null,
        decimal? minPrice = null,
        decimal? maxPrice = null,
        string? search = null,
        string? sortBy = null,
        int? travelers = null,
        CancellationToken cancellationToken = default);

    Task<TravelPackage?> GetWithDetailsAsync(int id, CancellationToken cancellationToken = default);
}

using Microsoft.EntityFrameworkCore;
using Travel.DAL.Data.Context;
using Travel.DAL.Entities;

namespace Travel.DAL.Repositories;

public class PackageRepository : GenericRepository<TravelPackage>, IPackageRepository
{
    private readonly ApplicationDbContext _context;

    public PackageRepository(ApplicationDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<(IReadOnlyList<TravelPackage> Items, int TotalCount)> SearchAsync(
        int pageNumber,
        int pageSize,
        int? destinationId = null,
        decimal? minPrice = null,
        decimal? maxPrice = null,
        string? search = null,
        string? sortBy = null,
        int? travelers = null,
        CancellationToken cancellationToken = default)
    {
        var query = _context.TravelPackages
            .AsNoTracking()
            .Include(p => p.Destination)
            .Where(p => p.IsActive)
            .AsQueryable();

        if (destinationId.HasValue)
            query = query.Where(p => p.DestinationId == destinationId.Value);

        if (minPrice.HasValue)
            query = query.Where(p => (p.DiscountPrice ?? p.Price) >= minPrice.Value);

        if (maxPrice.HasValue)
            query = query.Where(p => (p.DiscountPrice ?? p.Price) <= maxPrice.Value);

        if (travelers.HasValue)
            query = query.Where(p => p.AvailableSeats >= travelers.Value);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            query = query.Where(p =>
                p.Title.Contains(term) ||
                p.ArabicTitle.Contains(term) ||
                p.Destination.Name.Contains(term) ||
                p.Destination.City.Contains(term));
        }

        query = sortBy switch
        {
            "price_asc" => query.OrderBy(p => p.DiscountPrice ?? p.Price),
            "price_desc" => query.OrderByDescending(p => p.DiscountPrice ?? p.Price),
            "newest" => query.OrderByDescending(p => p.CreatedAt),
            // "rating" is intentionally left for Phase 8 once reviews are
            // aggregated via a projection — sorting on a related
            // collection's average here would defeat query efficiency.
            _ => query.OrderByDescending(p => p.IsFeatured).ThenByDescending(p => p.CreatedAt)
        };

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task<TravelPackage?> GetWithDetailsAsync(int id, CancellationToken cancellationToken = default)
        => await _context.TravelPackages
            .AsNoTracking()
            .Include(p => p.Destination)
            .Include(p => p.Images)
            .Include(p => p.Reviews.Where(r => r.IsApproved))
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
}

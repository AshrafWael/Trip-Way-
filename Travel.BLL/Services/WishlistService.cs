using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Travel.BLL.DTOs.Wishlist;
using Travel.BLL.Exceptions;
using Travel.BLL.Interfaces;
using Travel.DAL.Entities;
using Travel.DAL.UnitOfWork;

namespace Travel.BLL.Services;

public class WishlistService : IWishlistService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public WishlistService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<WishlistItemDto>> GetForUserAsync(string userId, CancellationToken cancellationToken = default)
    {
        var items = await _unitOfWork.Wishlists.Query()
            .Include(w => w.TravelPackage)
            .Where(w => w.UserId == userId)
            .OrderByDescending(w => w.AddedAt)
            .ToListAsync(cancellationToken);

        return _mapper.Map<IReadOnlyList<WishlistItemDto>>(items);
    }

    public async Task AddAsync(string userId, int packageId, CancellationToken cancellationToken = default)
    {
        var packageExists = await _unitOfWork.Packages.AnyAsync(p => p.Id == packageId && p.IsActive, cancellationToken);
        if (!packageExists)
            throw new NotFoundException(nameof(TravelPackage), packageId);

        var alreadyWishlisted = await _unitOfWork.Wishlists.AnyAsync(
            w => w.UserId == userId && w.TravelPackageId == packageId, cancellationToken);
        if (alreadyWishlisted)
            return;

        await _unitOfWork.Wishlists.AddAsync(
            new Wishlist { UserId = userId, TravelPackageId = packageId }, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task RemoveAsync(string userId, int packageId, CancellationToken cancellationToken = default)
    {
        var item = await _unitOfWork.Wishlists.Query(asNoTracking: false)
            .FirstOrDefaultAsync(w => w.UserId == userId && w.TravelPackageId == packageId, cancellationToken);

        if (item is null)
            return;

        _unitOfWork.Wishlists.Remove(item);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}

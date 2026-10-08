using Travel.BLL.DTOs.Wishlist;

namespace Travel.BLL.Interfaces;

public interface IWishlistService
{
    Task<IReadOnlyList<WishlistItemDto>> GetForUserAsync(string userId, CancellationToken cancellationToken = default);
    Task AddAsync(string userId, int packageId, CancellationToken cancellationToken = default);
    Task RemoveAsync(string userId, int packageId, CancellationToken cancellationToken = default);
}

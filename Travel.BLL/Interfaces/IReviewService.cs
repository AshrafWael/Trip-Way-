using Travel.BLL.DTOs.Reviews;

namespace Travel.BLL.Interfaces;

public interface IReviewService
{
    Task<IReadOnlyList<ReviewDto>> GetForPackageAsync(int packageId, CancellationToken cancellationToken = default);
    Task<ReviewDto> CreateAsync(string userId, CreateReviewDto dto, CancellationToken cancellationToken = default);

    // Admin moderation
    Task<IReadOnlyList<ReviewDto>> GetAllForAdminAsync(CancellationToken cancellationToken = default);
    Task ApproveAsync(int reviewId, CancellationToken cancellationToken = default);
    Task DeleteAsync(int reviewId, CancellationToken cancellationToken = default);
}

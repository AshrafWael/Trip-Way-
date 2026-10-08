using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Travel.BLL.DTOs.Reviews;
using Travel.BLL.Exceptions;
using Travel.BLL.Interfaces;
using Travel.DAL.Entities;
using Travel.DAL.UnitOfWork;

namespace Travel.BLL.Services;

public class ReviewService : IReviewService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public ReviewService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<ReviewDto>> GetForPackageAsync(int packageId, CancellationToken cancellationToken = default)
    {
        var reviews = await _unitOfWork.Reviews.Query()
            .Include(r => r.User)
            .Include(r => r.TravelPackage)
            .Where(r => r.TravelPackageId == packageId && r.IsApproved)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync(cancellationToken);

        return _mapper.Map<IReadOnlyList<ReviewDto>>(reviews);
    }

    public async Task<IReadOnlyList<ReviewDto>> GetAllForAdminAsync(CancellationToken cancellationToken = default)
    {
        var reviews = await _unitOfWork.Reviews.Query()
            .Include(r => r.User)
            .Include(r => r.TravelPackage)
            .OrderBy(r => r.IsApproved)
            .ThenByDescending(r => r.CreatedAt)
            .ToListAsync(cancellationToken);

        return _mapper.Map<IReadOnlyList<ReviewDto>>(reviews);
    }

    public async Task<ReviewDto> CreateAsync(string userId, CreateReviewDto dto, CancellationToken cancellationToken = default)
    {
        var packageExists = await _unitOfWork.Packages.AnyAsync(p => p.Id == dto.TravelPackageId, cancellationToken);
        if (!packageExists)
            throw new NotFoundException(nameof(TravelPackage), dto.TravelPackageId);

        // A customer may only review a package they've actually
        // completed a trip for.
        var hasCompletedBooking = await _unitOfWork.Bookings.AnyAsync(b =>
            b.UserId == userId &&
            b.TravelPackageId == dto.TravelPackageId &&
            b.Status == BookingStatus.Completed, cancellationToken);

        if (!hasCompletedBooking)
            throw new BusinessRuleException("You can only review packages from a completed trip.");

        var alreadyReviewed = await _unitOfWork.Reviews.AnyAsync(
            r => r.UserId == userId && r.TravelPackageId == dto.TravelPackageId, cancellationToken);
        if (alreadyReviewed)
            throw new BusinessRuleException("You have already reviewed this package.");

        var review = _mapper.Map<Review>(dto);
        review.UserId = userId;
        review.IsApproved = false; // moderated before it appears publicly

        await _unitOfWork.Reviews.AddAsync(review, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<ReviewDto>(review);
    }

    public async Task ApproveAsync(int reviewId, CancellationToken cancellationToken = default)
    {
        var review = await _unitOfWork.Reviews.GetByIdAsync(reviewId, cancellationToken)
            ?? throw new NotFoundException(nameof(Review), reviewId);

        review.IsApproved = true;
        _unitOfWork.Reviews.Update(review);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(int reviewId, CancellationToken cancellationToken = default)
    {
        var review = await _unitOfWork.Reviews.GetByIdAsync(reviewId, cancellationToken)
            ?? throw new NotFoundException(nameof(Review), reviewId);

        _unitOfWork.Reviews.Remove(review);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}

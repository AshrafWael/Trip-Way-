using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Travel.BLL.DTOs.Reviews;
using Travel.BLL.Exceptions;
using Travel.BLL.Mapping;
using Travel.BLL.Services;
using Travel.DAL.Entities;
using Travel.DAL.Repositories;
using Travel.DAL.UnitOfWork;
using Xunit;

namespace Travel.Tests.Services;

public class ReviewServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly Mock<IGenericRepository<Review>> _reviews = new();
    private readonly Mock<IGenericRepository<Booking>> _bookings = new();
    private readonly Mock<IPackageRepository> _packages = new();
    private readonly IMapper _mapper;

    public ReviewServiceTests()
    {
        _unitOfWork.Setup(u => u.Reviews).Returns(_reviews.Object);
        _unitOfWork.Setup(u => u.Bookings).Returns(_bookings.Object);
        _unitOfWork.Setup(u => u.Packages).Returns(_packages.Object);

        //var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
        //_mapper = config.CreateMapper();
        var loggerFactory = LoggerFactory.Create(builder => { });

        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<MappingProfile>();
        }, loggerFactory);

        _mapper = config.CreateMapper();
    }

    private ReviewService CreateSut() => new(_unitOfWork.Object, _mapper);
    [Fact]
    public async Task CreateAsync_Throws_WhenPackageDoesNotExist()
    {
        _packages.Setup(p => p.AnyAsync(It.IsAny<System.Linq.Expressions.Expression<Func<TravelPackage, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var sut = CreateSut();
        var dto = new CreateReviewDto { TravelPackageId = 1, Rating = 5 };

        var act = () => sut.CreateAsync("user-1", dto);

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task CreateAsync_Throws_WhenNoCompletedBooking()
    {
        _packages.Setup(p => p.AnyAsync(It.IsAny<System.Linq.Expressions.Expression<Func<TravelPackage, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _bookings.Setup(b => b.AnyAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Booking, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var sut = CreateSut();
        var dto = new CreateReviewDto { TravelPackageId = 1, Rating = 5 };

        var act = () => sut.CreateAsync("user-1", dto);

        await act.Should().ThrowAsync<BusinessRuleException>()
            .WithMessage("*completed trip*");
    }

    [Fact]
    public async Task CreateAsync_Throws_WhenAlreadyReviewed()
    {
        _packages.Setup(p => p.AnyAsync(It.IsAny<System.Linq.Expressions.Expression<Func<TravelPackage, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // First AnyAsync call (completed-booking check) -> true, second (already-reviewed check) -> true
        _bookings.Setup(b => b.AnyAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Booking, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _reviews.Setup(r => r.AnyAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Review, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var sut = CreateSut();
        var dto = new CreateReviewDto { TravelPackageId = 1, Rating = 5 };

        var act = () => sut.CreateAsync("user-1", dto);

        await act.Should().ThrowAsync<BusinessRuleException>()
            .WithMessage("*already reviewed*");
    }

    [Fact]
    public async Task CreateAsync_CreatesUnapprovedReview_WhenAllChecksPass()
    {
        _packages.Setup(p => p.AnyAsync(It.IsAny<System.Linq.Expressions.Expression<Func<TravelPackage, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _bookings.Setup(b => b.AnyAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Booking, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _reviews.Setup(r => r.AnyAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Review, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        Review? addedReview = null;
        _reviews.Setup(r => r.AddAsync(It.IsAny<Review>(), It.IsAny<CancellationToken>()))
            .Callback<Review, CancellationToken>((r, _) => addedReview = r)
            .Returns(Task.CompletedTask);

        var sut = CreateSut();
        var dto = new CreateReviewDto { TravelPackageId = 1, Rating = 4, Comment = "Great trip!" };

        await sut.CreateAsync("user-1", dto);

        addedReview.Should().NotBeNull();
        addedReview!.IsApproved.Should().BeFalse();
        addedReview.UserId.Should().Be("user-1");
        addedReview.Rating.Should().Be(4);
    }
}

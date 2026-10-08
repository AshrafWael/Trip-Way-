using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Travel.BLL.DTOs.Bookings;
using Travel.BLL.Exceptions;
using Travel.BLL.Mapping;
using Travel.BLL.Services;
using Travel.DAL.Entities;
using Travel.DAL.Repositories;
using Travel.DAL.UnitOfWork;
using Xunit;

namespace Travel.Tests.Services;

public class BookingServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly Mock<IGenericRepository<Booking>> _bookings = new();
    private readonly Mock<IPackageRepository> _packages = new();
    private readonly IMapper _mapper;

    public BookingServiceTests()
    {
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

    private BookingService CreateSut() => new(_unitOfWork.Object, _mapper);

    private static TravelPackage MakePackage(int availableSeats = 5, bool isActive = true, decimal price = 500m, decimal? discountPrice = null) => new()
    {
        Id = 1,
        Title = "Test Package",
        ArabicTitle = "رحلة تجريبية",
        Description = "desc",
        ArabicDescription = "وصف",
        DestinationId = 1,
        Price = price,
        DiscountPrice = discountPrice,
        AvailableSeats = availableSeats,
        MaxTravelers = 20,
        IsActive = isActive
    };

    [Fact]
    public async Task CreateAsync_Throws_WhenPackageDoesNotExist()
    {
        _packages.Setup(p => p.GetByIdAsync(It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((TravelPackage?)null);

        var sut = CreateSut();
        var dto = new CreateBookingDto { TravelPackageId = 99, TravelDate = DateTime.UtcNow.AddDays(5), NumberOfTravelers = 1 };

        var act = () => sut.CreateAsync("user-1", dto);

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task CreateAsync_Throws_WhenPackageIsInactive()
    {
        _packages.Setup(p => p.GetByIdAsync(It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(MakePackage(isActive: false));

        var sut = CreateSut();
        var dto = new CreateBookingDto { TravelPackageId = 1, TravelDate = DateTime.UtcNow.AddDays(5), NumberOfTravelers = 1 };

        var act = () => sut.CreateAsync("user-1", dto);

        await act.Should().ThrowAsync<BusinessRuleException>()
            .WithMessage("*no longer available*");
    }

    [Fact]
    public async Task CreateAsync_Throws_WhenNotEnoughSeats()
    {
        _packages.Setup(p => p.GetByIdAsync(It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(MakePackage(availableSeats: 2));

        var sut = CreateSut();
        var dto = new CreateBookingDto { TravelPackageId = 1, TravelDate = DateTime.UtcNow.AddDays(5), NumberOfTravelers = 3 };

        var act = () => sut.CreateAsync("user-1", dto);

        await act.Should().ThrowAsync<BusinessRuleException>()
            .WithMessage("*Only 2 seat(s) left*");
    }

    [Fact]
    public async Task CreateAsync_DecrementsAvailableSeats_AndUsesDiscountPrice_OnSuccess()
    {
        var package = MakePackage(availableSeats: 5, price: 500m, discountPrice: 400m);
        _packages.Setup(p => p.GetByIdAsync(It.IsAny<object>(), It.IsAny<CancellationToken>())).ReturnsAsync(package);

        Booking? addedBooking = null;
        _bookings.Setup(b => b.AddAsync(It.IsAny<Booking>(), It.IsAny<CancellationToken>()))
            .Callback<Booking, CancellationToken>((b, _) => addedBooking = b)
            .Returns(Task.CompletedTask);

        var sut = CreateSut();
        var dto = new CreateBookingDto { TravelPackageId = 1, TravelDate = DateTime.UtcNow.AddDays(5), NumberOfTravelers = 2 };

        await sut.CreateAsync("user-1", dto);

        package.AvailableSeats.Should().Be(3);
        addedBooking.Should().NotBeNull();
        addedBooking!.TotalPrice.Should().Be(800m); // discount price (400) * 2 travelers
        addedBooking.Status.Should().Be(BookingStatus.Pending);
        _unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CancelAsync_Throws_WhenBookingBelongsToAnotherUser()
    {
        var booking = new Booking { Id = 1, UserId = "owner", TravelPackageId = 1, Status = BookingStatus.Pending, NumberOfTravelers = 1 };
        _bookings.Setup(b => b.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(booking);

        var sut = CreateSut();

        var act = () => sut.CancelAsync(1, "someone-else");

        await act.Should().ThrowAsync<BusinessRuleException>();
    }

    [Theory]
    [InlineData(BookingStatus.Cancelled)]
    [InlineData(BookingStatus.Completed)]
    public async Task CancelAsync_Throws_WhenBookingAlreadyFinal(BookingStatus status)
    {
        var booking = new Booking { Id = 1, UserId = "user-1", TravelPackageId = 1, Status = status, NumberOfTravelers = 1 };
        _bookings.Setup(b => b.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(booking);

        var sut = CreateSut();

        var act = () => sut.CancelAsync(1, "user-1");

        await act.Should().ThrowAsync<BusinessRuleException>();
    }

    [Fact]
    public async Task CancelAsync_RestoresSeats_OnSuccess()
    {
        var booking = new Booking { Id = 1, UserId = "user-1", TravelPackageId = 1, Status = BookingStatus.Confirmed, NumberOfTravelers = 2 };
        var package = MakePackage(availableSeats: 3);

        _bookings.Setup(b => b.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(booking);
        _packages.Setup(p => p.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(package);

        var sut = CreateSut();
        await sut.CancelAsync(1, "user-1");

        booking.Status.Should().Be(BookingStatus.Cancelled);
        package.AvailableSeats.Should().Be(5);
    }
}

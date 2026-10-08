using FluentAssertions;
using Travel.BLL.DTOs.Bookings;
using Travel.BLL.Validators;
using Xunit;

namespace Travel.Tests.Validators;

public class CreateBookingDtoValidatorTests
{
    private readonly CreateBookingDtoValidator _validator = new();

    [Fact]
    public void Validate_Fails_WhenTravelDateIsInThePast()
    {
        var dto = new CreateBookingDto
        {
            TravelPackageId = 1,
            TravelDate = DateTime.UtcNow.AddDays(-1),
            NumberOfTravelers = 2
        };

        var result = _validator.Validate(dto);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(CreateBookingDto.TravelDate));
    }

    [Fact]
    public void Validate_Fails_WhenNumberOfTravelersIsZero()
    {
        var dto = new CreateBookingDto
        {
            TravelPackageId = 1,
            TravelDate = DateTime.UtcNow.AddDays(10),
            NumberOfTravelers = 0
        };

        var result = _validator.Validate(dto);

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Validate_Passes_ForValidFutureBooking()
    {
        var dto = new CreateBookingDto
        {
            TravelPackageId = 1,
            TravelDate = DateTime.UtcNow.AddDays(10),
            NumberOfTravelers = 2
        };

        var result = _validator.Validate(dto);

        result.IsValid.Should().BeTrue();
    }
}

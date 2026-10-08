using FluentAssertions;
using Travel.BLL.DTOs.Packages;
using Travel.BLL.Validators;
using Xunit;

namespace Travel.Tests.Validators;

public class CreatePackageDtoValidatorTests
{
    private readonly CreatePackageDtoValidator _validator = new();

    private static CreatePackageDto ValidDto() => new()
    {
        Title = "Cairo Highlights",
        ArabicTitle = "أبرز معالم القاهرة",
        Description = "A great trip",
        ArabicDescription = "رحلة رائعة",
        DestinationId = 1,
        DurationDays = 4,
        Price = 500m,
        MaxTravelers = 10,
        AvailableSeats = 10
    };

    [Fact]
    public void Validate_Passes_ForValidDto()
    {
        var result = _validator.Validate(ValidDto());
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_Fails_WhenDiscountPriceExceedsPrice()
    {
        var dto = ValidDto();
        dto.Price = 500m;
        dto.DiscountPrice = 600m;

        var result = _validator.Validate(dto);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(CreatePackageDto.DiscountPrice));
    }

    [Fact]
    public void Validate_Fails_WhenAvailableSeatsExceedsMaxTravelers()
    {
        var dto = ValidDto();
        dto.MaxTravelers = 5;
        dto.AvailableSeats = 10;

        var result = _validator.Validate(dto);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(CreatePackageDto.AvailableSeats));
    }

    [Fact]
    public void Validate_Fails_WhenPriceIsNegative()
    {
        var dto = ValidDto();
        dto.Price = -1m;

        var result = _validator.Validate(dto);

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Validate_Fails_WhenDestinationIdIsMissing()
    {
        var dto = ValidDto();
        dto.DestinationId = 0;

        var result = _validator.Validate(dto);

        result.IsValid.Should().BeFalse();
    }
}

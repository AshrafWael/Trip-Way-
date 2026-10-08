using FluentValidation;
using Travel.BLL.DTOs.Hotels;

namespace Travel.BLL.Validators;

public class CreateHotelDtoValidator : AbstractValidator<CreateHotelDto>
{
    public CreateHotelDtoValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.ArabicName).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Description).NotEmpty();
        RuleFor(x => x.ArabicDescription).NotEmpty();
        RuleFor(x => x.DestinationId).GreaterThan(0);
        RuleFor(x => x.Address).NotEmpty().MaximumLength(300);
        RuleFor(x => x.ArabicAddress).NotEmpty().MaximumLength(300);
        RuleFor(x => x.StarRating).InclusiveBetween(1, 5);
        RuleFor(x => x.PricePerNight).GreaterThanOrEqualTo(0);

        RuleFor(x => x.DiscountPricePerNight)
            .LessThanOrEqualTo(x => x.PricePerNight!.Value)
            .When(x => x.DiscountPricePerNight.HasValue && x.PricePerNight.HasValue)
            .WithMessage("Discount price cannot exceed the base price per night.");

        RuleFor(x => x.TotalRooms).GreaterThan(0);

        RuleFor(x => x.AvailableRooms)
            .LessThanOrEqualTo(x => x.TotalRooms)
            .WithMessage("Available rooms cannot exceed the total number of rooms.");
    }
}

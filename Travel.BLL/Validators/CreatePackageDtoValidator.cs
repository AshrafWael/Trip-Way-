using FluentValidation;
using Travel.BLL.DTOs.Packages;

namespace Travel.BLL.Validators;

public class CreatePackageDtoValidator : AbstractValidator<CreatePackageDto>
{
    public CreatePackageDtoValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
        RuleFor(x => x.ArabicTitle).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Description).NotEmpty();
        RuleFor(x => x.ArabicDescription).NotEmpty();
        RuleFor(x => x.DestinationId).GreaterThan(0);
        RuleFor(x => x.DurationDays).InclusiveBetween(1, 60);
        RuleFor(x => x.Price).GreaterThanOrEqualTo(0);

        RuleFor(x => x.DiscountPrice)
            .LessThanOrEqualTo(x => x.Price!.Value)
            .When(x => x.DiscountPrice.HasValue && x.Price.HasValue)
            .WithMessage("Discount price cannot exceed the original price.");

        RuleFor(x => x.MaxTravelers).GreaterThan(0);

        RuleFor(x => x.AvailableSeats)
            .LessThanOrEqualTo(x => x.MaxTravelers)
            .WithMessage("Available seats cannot exceed the maximum number of travelers.");
    }
}

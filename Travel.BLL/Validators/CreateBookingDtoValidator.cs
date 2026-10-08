using FluentValidation;
using Travel.BLL.DTOs.Bookings;

namespace Travel.BLL.Validators;

public class CreateBookingDtoValidator : AbstractValidator<CreateBookingDto>
{
    public CreateBookingDtoValidator()
    {
        RuleFor(x => x.TravelPackageId).GreaterThan(0);

        RuleFor(x => x.TravelDate)
            .GreaterThan(DateTime.UtcNow.Date)
            .WithMessage("Travel date cannot be in the past.");

        RuleFor(x => x.NumberOfTravelers).InclusiveBetween(1, 50);
    }
}

using FluentValidation;
using Travel.BLL.DTOs.Hotels;

namespace Travel.BLL.Validators;

public class CreateHotelBookingDtoValidator : AbstractValidator<CreateHotelBookingDto>
{
    public CreateHotelBookingDtoValidator()
    {
        RuleFor(x => x.HotelId).GreaterThan(0);

        RuleFor(x => x.CheckInDate)
            .GreaterThanOrEqualTo(DateTime.UtcNow.Date)
            .WithMessage("Check-in date cannot be in the past.");

        RuleFor(x => x.CheckOutDate)
            .GreaterThan(x => x.CheckInDate)
            .WithMessage("Check-out date must be after the check-in date.");

        RuleFor(x => x.NumberOfRooms).InclusiveBetween(1, 100);
        RuleFor(x => x.NumberOfGuests).InclusiveBetween(1, 500);
    }
}

using FluentValidation;
using Travel.BLL.DTOs.Destinations;

namespace Travel.BLL.Validators;

public class CreateDestinationDtoValidator : AbstractValidator<CreateDestinationDto>
{
    public CreateDestinationDtoValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(150);
        RuleFor(x => x.ArabicName).NotEmpty().MaximumLength(150);
        RuleFor(x => x.Description).NotEmpty().MaximumLength(4000);
        RuleFor(x => x.ArabicDescription).NotEmpty().MaximumLength(4000);
        RuleFor(x => x.Country).NotEmpty().MaximumLength(100);
        RuleFor(x => x.City).NotEmpty().MaximumLength(100);
    }
}

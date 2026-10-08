using FluentValidation;
using Travel.BLL.DTOs.Contact;

namespace Travel.BLL.Validators;

public class CreateContactMessageDtoValidator : AbstractValidator<CreateContactMessageDto>
{
    public CreateContactMessageDtoValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(150);
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Subject).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Message).NotEmpty().MaximumLength(4000);
    }
}

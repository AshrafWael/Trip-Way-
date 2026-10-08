using FluentAssertions;
using Travel.BLL.DTOs.Auth;
using Travel.BLL.Validators;
using Xunit;

namespace Travel.Tests.Validators;

public class RegisterDtoValidatorTests
{
    private readonly RegisterDtoValidator _validator = new();

    private static RegisterDto ValidDto() => new()
    {
        FullName = "Ahmed Ali",
        Email = "ahmed@example.com",
        Password = "Str0ngPass",
        ConfirmPassword = "Str0ngPass"
    };

    [Fact]
    public void Validate_Passes_ForValidRegistration()
    {
        var result = _validator.Validate(ValidDto());
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_Fails_WhenPasswordsDoNotMatch()
    {
        var dto = ValidDto();
        dto.ConfirmPassword = "SomethingElse1";

        var result = _validator.Validate(dto);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(RegisterDto.ConfirmPassword));
    }

    [Theory]
    [InlineData("short1A")]      // < 8 chars
    [InlineData("alllowercase1")] // no uppercase
    [InlineData("NoDigitsHere")]  // no digit
    public void Validate_Fails_ForWeakPasswords(string weakPassword)
    {
        var dto = ValidDto();
        dto.Password = weakPassword;
        dto.ConfirmPassword = weakPassword;

        var result = _validator.Validate(dto);

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Validate_Fails_ForInvalidEmail()
    {
        var dto = ValidDto();
        dto.Email = "not-an-email";

        var result = _validator.Validate(dto);

        result.IsValid.Should().BeFalse();
    }
}

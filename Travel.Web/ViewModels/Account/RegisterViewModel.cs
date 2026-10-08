using System.ComponentModel.DataAnnotations;

namespace Travel.Web.ViewModels.Account;

public class RegisterViewModel
{
    [Required, Display(Name = "FullName")]
    public string FullName { get; set; } = string.Empty;

    [Required, EmailAddress, Display(Name = "Email")]
    public string Email { get; set; } = string.Empty;

    [Display(Name = "PhoneNumber")]
    public string? PhoneNumber { get; set; }

    [Required, DataType(DataType.Password), Display(Name = "Password")]
    public string Password { get; set; } = string.Empty;

    [Required, DataType(DataType.Password), Compare(nameof(Password)), Display(Name = "ConfirmPassword")]
    public string ConfirmPassword { get; set; } = string.Empty;
}

using System.ComponentModel.DataAnnotations;

namespace Travel.Web.ViewModels.Home;

public class ContactViewModel
{
    [Required, MaxLength(150), Display(Name = "Name")]
    public string Name { get; set; } = string.Empty;

    [Required, EmailAddress, Display(Name = "Email")]
    public string Email { get; set; } = string.Empty;

    [Display(Name = "Phone")]
    public string? Phone { get; set; }

    [Required, MaxLength(200), Display(Name = "Subject")]
    public string Subject { get; set; } = string.Empty;

    [Required, MaxLength(4000), Display(Name = "Message")]
    public string Message { get; set; } = string.Empty;
}

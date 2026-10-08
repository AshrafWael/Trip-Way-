using System.ComponentModel.DataAnnotations;

namespace Travel.Web.ViewModels.Contact;

public class ContactFormViewModel
{
    [Required, Display(Name = "Name")]
    public string Name { get; set; } = string.Empty;

    [Required, EmailAddress, Display(Name = "Email")]
    public string Email { get; set; } = string.Empty;

    [Display(Name = "Phone")]
    public string? Phone { get; set; }

    [Required, Display(Name = "Subject")]
    public string Subject { get; set; } = string.Empty;

    [Required, Display(Name = "Message")]
    public string Message { get; set; } = string.Empty;
}

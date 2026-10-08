using System.ComponentModel.DataAnnotations;

namespace Travel.BLL.DTOs.Contact;

public class SubscribeNewsletterDto
{
    [Required, EmailAddress, MaxLength(256)]
    public string Email { get; set; } = string.Empty;
}

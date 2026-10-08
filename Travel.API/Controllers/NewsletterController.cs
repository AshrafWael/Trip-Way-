using Microsoft.AspNetCore.Mvc;
using Travel.BLL.DTOs.Common;
using Travel.BLL.DTOs.Contact;
using Travel.BLL.Interfaces;

namespace Travel.API.Controllers;

[ApiController]
[Route("api/newsletter")]
public class NewsletterController : ControllerBase
{
    private readonly INewsletterService _newsletterService;

    public NewsletterController(INewsletterService newsletterService)
    {
        _newsletterService = newsletterService;
    }

    [HttpPost("subscribe")]
    public async Task<ActionResult<ApiResponse<object>>> Subscribe(SubscribeNewsletterDto dto, CancellationToken cancellationToken)
    {
        var isNewSubscription = await _newsletterService.SubscribeAsync(dto.Email, cancellationToken);

        var message = isNewSubscription
            ? "تم الاشتراك بنجاح."
            : "هذا البريد مشترك بالفعل.";

        return Ok(ApiResponse<object>.Ok(new { }, message));
    }
}

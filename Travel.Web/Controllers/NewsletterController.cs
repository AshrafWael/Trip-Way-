using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Travel.Web.Resources;
using Travel.Web.Services.ApiClient;

namespace Travel.Web.Controllers;

public class NewsletterController : Controller
{
    private readonly ITravelApiClient _apiClient;
    private readonly IStringLocalizer<SharedResource> _localizer;

    public NewsletterController(ITravelApiClient apiClient, IStringLocalizer<SharedResource> localizer)
    {
        _apiClient = apiClient;
        _localizer = localizer;
    }

    /// <summary>Called via fetch() from the homepage newsletter form -- see wwwroot/js/site.js.</summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Subscribe(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return Json(new { success = false, message = _localizer["NewsletterInvalidEmail"].Value });

        try
        {
            var message = await _apiClient.SubscribeToNewsletterAsync(email);
            return Json(new { success = true, message });
        }
        catch (ApiException ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Travel.BLL.DTOs.Contact;
using Travel.Web.Resources;
using Travel.Web.Services.ApiClient;
using Travel.Web.ViewModels.Home;

namespace Travel.Web.Controllers;

public class ContactController : Controller
{
    private readonly ITravelApiClient _apiClient;
    private readonly IStringLocalizer<SharedResource> _localizer;

    public ContactController(ITravelApiClient apiClient, IStringLocalizer<SharedResource> localizer)
    {
        _apiClient = apiClient;
        _localizer = localizer;
    }

    public async Task<IActionResult> Index()
    {
        await SetSettingsViewBagAsync();
        return View(new ContactViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(ContactViewModel model)
    {
        if (!ModelState.IsValid)
        {
            await SetSettingsViewBagAsync();
            return View(model);
        }

        try
        {
            await _apiClient.SubmitContactMessageAsync(new CreateContactMessageDto
            {
                Name = model.Name,
                Email = model.Email,
                Phone = model.Phone,
                Subject = model.Subject,
                Message = model.Message
            });

            TempData["Success"] = _localizer["ContactSent"].Value;
            return RedirectToAction(nameof(Index));
        }
        catch (ApiException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            await SetSettingsViewBagAsync();
            return View(model);
        }
    }

    private async Task SetSettingsViewBagAsync()
    {
        try
        {
            ViewBag.Settings = await _apiClient.GetSiteSettingsAsync();
        }
        catch (ApiException)
        {
            ViewBag.Settings = null;
        }
    }
}

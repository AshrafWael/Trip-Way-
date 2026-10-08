using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Travel.BLL.DTOs.Settings;
using Travel.Web.Helpers;
using Travel.Web.Resources;
using Travel.Web.Services.ApiClient;

namespace Travel.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class SettingsController : Controller
{
    private readonly ITravelApiClient _apiClient;
    private readonly IStringLocalizer<SharedResource> _localizer;
    private readonly IWebHostEnvironment _env;

    public SettingsController(ITravelApiClient apiClient, IStringLocalizer<SharedResource> localizer, IWebHostEnvironment env)
    {
        _apiClient = apiClient;
        _localizer = localizer;
        _env = env;
    }

    public async Task<IActionResult> Index()
    {
        var settings = await _apiClient.GetSiteSettingsAsync();
        return View(new UpdateSiteSettingsDto
        {
            HeroTitle = settings.HeroTitle,
            ArabicHeroTitle = settings.ArabicHeroTitle,
            HeroSubtitle = settings.HeroSubtitle,
            ArabicHeroSubtitle = settings.ArabicHeroSubtitle,
            HeroImageUrl = settings.HeroImageUrl,
            ContactPhone = settings.ContactPhone,
            ContactEmail = settings.ContactEmail,
            ContactAddress = settings.ContactAddress,
            ArabicContactAddress = settings.ArabicContactAddress,
            ContactHours = settings.ContactHours,
            ArabicContactHours = settings.ArabicContactHours,
            WhatsAppNumber = settings.WhatsAppNumber
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(UpdateSiteSettingsDto dto, IFormFile? heroImageFile)
    {
        if (!ModelState.IsValid)
            return View(dto);

        try
        {
            var uploadedUrl = await FileUploadHelper.SaveImageAsync(heroImageFile, "settings", _env, _localizer);
            if (uploadedUrl is not null) dto.HeroImageUrl = uploadedUrl;

            await _apiClient.UpdateSiteSettingsAsync(dto);
            TempData["Success"] = _localizer["AdminMsgSettingsUpdated"].Value;
            return RedirectToAction(nameof(Index));
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return View(dto);
        }
        catch (ApiException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return View(dto);
        }
    }
}

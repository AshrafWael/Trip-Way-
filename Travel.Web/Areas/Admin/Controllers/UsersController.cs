using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Travel.Web.Resources;
using Travel.Web.Services.ApiClient;

namespace Travel.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class UsersController : Controller
{
    private readonly ITravelApiClient _apiClient;
    private readonly IStringLocalizer<SharedResource> _localizer;

    public UsersController(ITravelApiClient apiClient, IStringLocalizer<SharedResource> localizer)
    {
        _apiClient = apiClient;
        _localizer = localizer;
    }

    public async Task<IActionResult> Index()
    {
        var users = await _apiClient.GetUsersAsync();
        return View(users);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Lock(string id)
    {
        try
        {
            await _apiClient.LockUserAsync(id);
            TempData["Success"] = _localizer["AccountLocked"].Value;
        }
        catch (ApiException ex)
        {
            TempData["Error"] = ex.Message;
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Unlock(string id)
    {
        try
        {
            await _apiClient.UnlockUserAsync(id);
            TempData["Success"] = _localizer["AccountUnlocked"].Value;
        }
        catch (ApiException ex)
        {
            TempData["Error"] = ex.Message;
        }

        return RedirectToAction(nameof(Index));
    }
}

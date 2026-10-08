using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Travel.Web.Resources;
using Travel.Web.Services.ApiClient;

namespace Travel.Web.Controllers;

[Authorize]
public class WishlistController : Controller
{
    private readonly ITravelApiClient _apiClient;
    private readonly IStringLocalizer<SharedResource> _localizer;

    public WishlistController(ITravelApiClient apiClient, IStringLocalizer<SharedResource> localizer)
    {
        _apiClient = apiClient;
        _localizer = localizer;
    }

    public async Task<IActionResult> Index()
    {
        var items = await _apiClient.GetMyWishlistAsync();
        return View(items);
    }

    /// <summary>Called via fetch() from the package details page -- see wwwroot/js/site.js.</summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Add(int packageId)
    {
        try
        {
            await _apiClient.AddToWishlistAsync(packageId);
            return Json(new { success = true, message = _localizer["WishlistAdded"].Value });
        }
        catch (ApiException ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Remove(int packageId)
    {
        try
        {
            await _apiClient.RemoveFromWishlistAsync(packageId);

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                return Json(new { success = true });

            TempData["Success"] = _localizer["WishlistRemoved"].Value;
        }
        catch (ApiException ex)
        {
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                return Json(new { success = false, message = ex.Message });

            TempData["Error"] = ex.Message;
        }

        return RedirectToAction(nameof(Index));
    }
}

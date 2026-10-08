using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Travel.Web.Resources;
using Travel.Web.Services.ApiClient;

namespace Travel.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class ReviewsController : Controller
{
    private readonly ITravelApiClient _apiClient;
    private readonly IStringLocalizer<SharedResource> _localizer;

    public ReviewsController(ITravelApiClient apiClient, IStringLocalizer<SharedResource> localizer)
    {
        _apiClient = apiClient;
        _localizer = localizer;
    }

    public async Task<IActionResult> Index()
    {
        var reviews = await _apiClient.GetAllReviewsForAdminAsync();
        return View(reviews);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Approve(int id)
    {
        try
        {
            await _apiClient.ApproveReviewAsync(id);
            TempData["Success"] = _localizer["ReviewApproved"].Value;
        }
        catch (ApiException ex)
        {
            TempData["Error"] = ex.Message;
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _apiClient.DeleteReviewAsync(id);
            TempData["Success"] = _localizer["ReviewDeleted"].Value;
        }
        catch (ApiException ex)
        {
            TempData["Error"] = ex.Message;
        }

        return RedirectToAction(nameof(Index));
    }
}

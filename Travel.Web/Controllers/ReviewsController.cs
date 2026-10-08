using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Travel.BLL.DTOs.Reviews;
using Travel.Web.Extensions;
using Travel.Web.Resources;
using Travel.Web.Services.ApiClient;

namespace Travel.Web.Controllers;

[Authorize]
public class ReviewsController : Controller
{
    private readonly ITravelApiClient _apiClient;
    private readonly IStringLocalizer<SharedResource> _localizer;

    public ReviewsController(ITravelApiClient apiClient, IStringLocalizer<SharedResource> localizer)
    {
        _apiClient = apiClient;
        _localizer = localizer;
    }

    [HttpGet]
    public async Task<IActionResult> Create(int packageId)
    {
        var package = await _apiClient.GetPackageAsync(packageId);
        ViewBag.PackageTitle = package.L();
        return View(new CreateReviewDto { TravelPackageId = packageId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateReviewDto dto)
    {
        if (!ModelState.IsValid)
            return View(dto);

        try
        {
            await _apiClient.SubmitReviewAsync(dto.TravelPackageId, dto);
            TempData["Success"] = _localizer["ReviewSubmitted"].Value;
            return RedirectToAction("Index", "Bookings");
        }
        catch (ApiException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return View(dto);
        }
    }
}

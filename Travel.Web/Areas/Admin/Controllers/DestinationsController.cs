using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Travel.BLL.DTOs.Destinations;
using Travel.Web.Helpers;
using Travel.Web.Resources;
using Travel.Web.Services.ApiClient;

namespace Travel.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class DestinationsController : Controller
{
    private readonly ITravelApiClient _apiClient;
    private readonly IStringLocalizer<SharedResource> _localizer;
    private readonly IWebHostEnvironment _env;

    public DestinationsController(ITravelApiClient apiClient, IStringLocalizer<SharedResource> localizer, IWebHostEnvironment env)
    {
        _apiClient = apiClient;
        _localizer = localizer;
        _env = env;
    }

    public async Task<IActionResult> Index()
    {
        var destinations = await _apiClient.GetDestinationsAsync(includeInactive: true);
        return View(destinations);
    }

    public IActionResult Create() => View(new CreateDestinationDto());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateDestinationDto dto, IFormFile? imageFile)
    {
        if (!ModelState.IsValid)
            return View(dto);

        try
        {
            // A device-uploaded photo takes priority over a pasted URL;
            // see FileUploadHelper for why this is the whole upload
            // pipeline (no dedicated file-storage service needed).
            var uploadedUrl = await FileUploadHelper.SaveImageAsync(imageFile, "destinations", _env, _localizer);
            if (uploadedUrl is not null) dto.ImageUrl = uploadedUrl;

            await _apiClient.CreateDestinationAsync(dto);
            TempData["Success"] = _localizer["DestinationCreated"].Value;
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

    public async Task<IActionResult> Edit(int id)
    {
        var destination = await _apiClient.GetDestinationAsync(id);
        var dto = new UpdateDestinationDto
        {
            Name = destination.Name,
            ArabicName = destination.ArabicName,
            Description = destination.Description,
            ArabicDescription = destination.ArabicDescription,
            Country = destination.Country,
            City = destination.City,
            ImageUrl = destination.ImageUrl,
            IsFeatured = destination.IsFeatured,
            IsActive = destination.IsActive
        };

        ViewBag.DestinationId = id;
        return View(dto);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, UpdateDestinationDto dto, IFormFile? imageFile)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.DestinationId = id;
            return View(dto);
        }

        try
        {
            var uploadedUrl = await FileUploadHelper.SaveImageAsync(imageFile, "destinations", _env, _localizer);
            if (uploadedUrl is not null) dto.ImageUrl = uploadedUrl;

            await _apiClient.UpdateDestinationAsync(id, dto);
            TempData["Success"] = _localizer["DestinationUpdated"].Value;
            return RedirectToAction(nameof(Index));
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            ViewBag.DestinationId = id;
            return View(dto);
        }
        catch (ApiException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            ViewBag.DestinationId = id;
            return View(dto);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _apiClient.DeleteDestinationAsync(id);
            TempData["Success"] = _localizer["DestinationDeactivated"].Value;
        }
        catch (ApiException ex)
        {
            TempData["Error"] = ex.Message;
        }

        return RedirectToAction(nameof(Index));
    }
}


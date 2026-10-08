using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Travel.BLL.DTOs.Packages;
using Travel.Web.Helpers;
using Travel.Web.Resources;
using Travel.Web.Services.ApiClient;

namespace Travel.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class PackagesController : Controller
{
    private readonly ITravelApiClient _apiClient;
    private readonly IStringLocalizer<SharedResource> _localizer;
    private readonly IWebHostEnvironment _env;

    public PackagesController(ITravelApiClient apiClient, IStringLocalizer<SharedResource> localizer, IWebHostEnvironment env)
    {
        _apiClient = apiClient;
        _localizer = localizer;
        _env = env;
    }

    public async Task<IActionResult> Index()
    {
        var packages = await _apiClient.GetAllPackagesForAdminAsync();
        return View(packages);
    }

    public async Task<IActionResult> Create()
    {
        ViewBag.Destinations = await _apiClient.GetDestinationsAsync(includeInactive: false);
        return View(new CreatePackageDto());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequestSizeLimit(300_000_000)]
    [RequestFormLimits(MultipartBodyLengthLimit = 300_000_000)]
    public async Task<IActionResult> Create(
        CreatePackageDto dto,
        string? imageUrlsRaw,
        IFormFile? mainImageFile,
        IFormFile? videoFile,
        List<IFormFile>? galleryFiles)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Destinations = await _apiClient.GetDestinationsAsync(includeInactive: false);
            return View(dto);
        }

        try
        {
            // Device-uploaded photos/video take priority over pasted URLs.
            var uploadedMainImage = await FileUploadHelper.SaveImageAsync(mainImageFile, "packages", _env, _localizer);
            if (uploadedMainImage is not null) dto.MainImageUrl = uploadedMainImage;

            var uploadedVideo = await FileUploadHelper.SaveVideoAsync(videoFile, "packages", _env, _localizer);
            if (uploadedVideo is not null) dto.VideoUrl = uploadedVideo;

            var galleryUrls = ParseImageUrls(imageUrlsRaw);
            galleryUrls.AddRange(await FileUploadHelper.SaveImagesAsync(galleryFiles, "packages", _env, _localizer));
            dto.ImageUrls = galleryUrls;

            await _apiClient.CreatePackageAsync(dto);
            TempData["Success"] = _localizer["PackageCreated"].Value;
            return RedirectToAction(nameof(Index));
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            ViewBag.Destinations = await _apiClient.GetDestinationsAsync(includeInactive: false);
            return View(dto);
        }
        catch (ApiException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            ViewBag.Destinations = await _apiClient.GetDestinationsAsync(includeInactive: false);
            return View(dto);
        }
    }

    public async Task<IActionResult> Edit(int id)
    {
        var package = await _apiClient.GetPackageAsync(id);
        ViewBag.Destinations = await _apiClient.GetDestinationsAsync(includeInactive: false);
        ViewBag.PackageId = id;
        ViewBag.ImageUrlsRaw = string.Join("\n", package.Images.Select(i => i.ImageUrl));
        ViewBag.CurrentGalleryImages = package.Images.Select(i => i.ImageUrl).ToList();

        return View(new UpdatePackageDto
        {
            Title = package.Title,
            ArabicTitle = package.ArabicTitle,
            Description = package.Description,
            ArabicDescription = package.ArabicDescription,
            DestinationId = package.DestinationId,
            DurationDays = package.DurationDays,
            DurationNights = package.DurationNights,
            Price = package.Price,
            DiscountPrice = package.DiscountPrice,
            MaxTravelers = package.MaxTravelers,
            AvailableSeats = package.AvailableSeats,
            MainImageUrl = package.MainImageUrl,
            VideoUrl = package.VideoUrl,
            IsFeatured = package.IsFeatured,
            IsActive = package.IsActive
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequestSizeLimit(300_000_000)]
    [RequestFormLimits(MultipartBodyLengthLimit = 300_000_000)]
    public async Task<IActionResult> Edit(
        int id,
        UpdatePackageDto dto,
        string? imageUrlsRaw,
        IFormFile? mainImageFile,
        IFormFile? videoFile,
        List<IFormFile>? galleryFiles)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Destinations = await _apiClient.GetDestinationsAsync(includeInactive: false);
            ViewBag.PackageId = id;
            ViewBag.ImageUrlsRaw = imageUrlsRaw;
            return View(dto);
        }

        try
        {
            var uploadedMainImage = await FileUploadHelper.SaveImageAsync(mainImageFile, "packages", _env, _localizer);
            if (uploadedMainImage is not null) dto.MainImageUrl = uploadedMainImage;

            var uploadedVideo = await FileUploadHelper.SaveVideoAsync(videoFile, "packages", _env, _localizer);
            if (uploadedVideo is not null) dto.VideoUrl = uploadedVideo;

            var galleryUrls = ParseImageUrls(imageUrlsRaw);
            galleryUrls.AddRange(await FileUploadHelper.SaveImagesAsync(galleryFiles, "packages", _env, _localizer));

            await _apiClient.UpdatePackageAsync(id, dto);
            await _apiClient.UpdatePackageImagesAsync(id, galleryUrls);
            TempData["Success"] = _localizer["PackageUpdated"].Value;
            return RedirectToAction(nameof(Index));
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            ViewBag.Destinations = await _apiClient.GetDestinationsAsync(includeInactive: false);
            ViewBag.PackageId = id;
            ViewBag.ImageUrlsRaw = imageUrlsRaw;
            return View(dto);
        }
        catch (ApiException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            ViewBag.Destinations = await _apiClient.GetDestinationsAsync(includeInactive: false);
            ViewBag.PackageId = id;
            ViewBag.ImageUrlsRaw = imageUrlsRaw;
            return View(dto);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _apiClient.DeletePackageAsync(id);
            TempData["Success"] = _localizer["PackageDeactivated"].Value;
        }
        catch (ApiException ex)
        {
            TempData["Error"] = ex.Message;
        }

        return RedirectToAction(nameof(Index));
    }

    private static List<string> ParseImageUrls(string? raw) =>
        string.IsNullOrWhiteSpace(raw)
            ? new List<string>()
            : raw.Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList();
}

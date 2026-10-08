using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Travel.BLL.DTOs.Blog;
using Travel.Web.Helpers;
using Travel.Web.Resources;
using Travel.Web.Services.ApiClient;

namespace Travel.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class BlogController : Controller
{
    private readonly ITravelApiClient _apiClient;
    private readonly IStringLocalizer<SharedResource> _localizer;
    private readonly IWebHostEnvironment _env;

    public BlogController(ITravelApiClient apiClient, IStringLocalizer<SharedResource> localizer, IWebHostEnvironment env)
    {
        _apiClient = apiClient;
        _localizer = localizer;
        _env = env;
    }

    public async Task<IActionResult> Index()
    {
        var posts = await _apiClient.GetAllBlogPostsForAdminAsync();
        return View(posts);
    }

    public IActionResult Create() => View(new CreateBlogPostDto());

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequestSizeLimit(300_000_000)]
    [RequestFormLimits(MultipartBodyLengthLimit = 300_000_000)]
    public async Task<IActionResult> Create(
        CreateBlogPostDto dto,
        string? imageUrlsRaw,
        IFormFile? imageFile,
        List<IFormFile>? galleryFiles)
    {
        if (!ModelState.IsValid)
            return View(dto);

        try
        {
            var uploadedUrl = await FileUploadHelper.SaveImageAsync(imageFile, "blog", _env, _localizer);
            if (uploadedUrl is not null) dto.ImageUrl = uploadedUrl;

            var galleryUrls = ParseImageUrls(imageUrlsRaw);
            galleryUrls.AddRange(await FileUploadHelper.SaveImagesAsync(galleryFiles, "blog", _env, _localizer));
            dto.ImageUrls = galleryUrls;

            await _apiClient.CreateBlogPostAsync(dto);
            TempData["Success"] = _localizer["PostPublished"].Value;
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
        var post = await _apiClient.GetBlogPostAsync(id);
        ViewBag.PostId = id;
        ViewBag.ImageUrlsRaw = string.Join("\n", post.Images.Select(i => i.ImageUrl));
        ViewBag.CurrentGalleryImages = post.Images.Select(i => i.ImageUrl).ToList();

        return View(new UpdateBlogPostDto
        {
            Title = post.Title,
            ArabicTitle = post.ArabicTitle,
            Subtitle = post.Subtitle,
            ArabicSubtitle = post.ArabicSubtitle,
            Content = post.Content,
            ArabicContent = post.ArabicContent,
            ImageUrl = post.ImageUrl,
            IsPublished = post.IsPublished
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequestSizeLimit(300_000_000)]
    [RequestFormLimits(MultipartBodyLengthLimit = 300_000_000)]
    public async Task<IActionResult> Edit(
        int id,
        UpdateBlogPostDto dto,
        string? imageUrlsRaw,
        IFormFile? imageFile,
        List<IFormFile>? galleryFiles)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.PostId = id;
            ViewBag.ImageUrlsRaw = imageUrlsRaw;
            return View(dto);
        }

        try
        {
            var uploadedUrl = await FileUploadHelper.SaveImageAsync(imageFile, "blog", _env, _localizer);
            if (uploadedUrl is not null) dto.ImageUrl = uploadedUrl;

            var galleryUrls = ParseImageUrls(imageUrlsRaw);
            galleryUrls.AddRange(await FileUploadHelper.SaveImagesAsync(galleryFiles, "blog", _env, _localizer));

            await _apiClient.UpdateBlogPostAsync(id, dto);
            await _apiClient.UpdateBlogPostImagesAsync(id, galleryUrls);
            TempData["Success"] = _localizer["PostUpdated"].Value;
            return RedirectToAction(nameof(Index));
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            ViewBag.PostId = id;
            ViewBag.ImageUrlsRaw = imageUrlsRaw;
            return View(dto);
        }
        catch (ApiException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            ViewBag.PostId = id;
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
            await _apiClient.DeleteBlogPostAsync(id);
            TempData["Success"] = _localizer["PostDeleted"].Value;
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

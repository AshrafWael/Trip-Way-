using Microsoft.Extensions.Localization;

namespace Travel.Web.Helpers;

/// <summary>
/// Every image/video field in this project (Destination.ImageUrl,
/// TravelPackage.MainImageUrl/VideoUrl, PackageImage.ImageUrl,
/// BlogPost.ImageUrl) is just a URL string -- there was never a
/// dedicated upload pipeline, so admins had to paste a link to an
/// already-hosted file. This helper saves a browser-uploaded file
/// (IFormFile) to wwwroot/uploads/{subFolder}/ under a random file
/// name and returns the site-relative URL to store in that same
/// string field. Because Travel.Web already serves wwwroot via
/// UseStaticFiles(), the returned URL works immediately -- no new
/// infrastructure needed beyond this helper.
/// </summary>
public static class FileUploadHelper
{
    private static readonly string[] ImageExtensions = { ".jpg", ".jpeg", ".png", ".webp", ".gif" };
    private static readonly string[] VideoExtensions = { ".mp4", ".webm", ".mov", ".m4v" };

    public const long MaxImageBytes = 10 * 1024 * 1024;   // 10 MB per photo
    public const long MaxVideoBytes = 200 * 1024 * 1024;  // 200 MB per video

    public static bool IsAllowedImage(IFormFile file) =>
        ImageExtensions.Contains(Path.GetExtension(file.FileName).ToLowerInvariant()) && file.Length <= MaxImageBytes;

    public static bool IsAllowedVideo(IFormFile file) =>
        VideoExtensions.Contains(Path.GetExtension(file.FileName).ToLowerInvariant()) && file.Length <= MaxVideoBytes;

    /// <summary>Saves one image. Returns null if no file was chosen. Throws InvalidOperationException if the file is rejected.</summary>
    public static Task<string?> SaveImageAsync(IFormFile? file, string subFolder, IWebHostEnvironment env, IStringLocalizer localizer) =>
        SaveAsync(file, subFolder, env, IsAllowedImage, localizer["InvalidImage"]);

    /// <summary>Saves one video. Returns null if no file was chosen. Throws InvalidOperationException if the file is rejected.</summary>
    public static Task<string?> SaveVideoAsync(IFormFile? file, string subFolder, IWebHostEnvironment env, IStringLocalizer localizer) =>
        SaveAsync(file, subFolder, env, IsAllowedVideo, localizer["InvalidVideo"]);

    /// <summary>Saves every valid image in the list (invalid ones are skipped) and returns their URLs in order.</summary>
    public static async Task<List<string>> SaveImagesAsync(IEnumerable<IFormFile>? files, string subFolder, IWebHostEnvironment env, IStringLocalizer localizer)
    {
        var urls = new List<string>();
        if (files is null) return urls;

        foreach (var file in files.Where(f => f.Length > 0))
        {
            var url = await SaveImageAsync(file, subFolder, env, localizer);
            if (url is not null) urls.Add(url);
        }

        return urls;
    }

    private static async Task<string?> SaveAsync(IFormFile? file, string subFolder, IWebHostEnvironment env, Func<IFormFile, bool> isAllowed, string rejectionMessage)
    {
        if (file is null || file.Length == 0) return null;
        if (!isAllowed(file)) throw new InvalidOperationException(rejectionMessage);

        var uploadsRoot = Path.Combine(env.WebRootPath, "uploads", subFolder);
        Directory.CreateDirectory(uploadsRoot);

        var fileName = $"{Guid.NewGuid():N}{Path.GetExtension(file.FileName).ToLowerInvariant()}";
        var fullPath = Path.Combine(uploadsRoot, fileName);

        await using var stream = new FileStream(fullPath, FileMode.Create);
        await file.CopyToAsync(stream);

        return $"/uploads/{subFolder}/{fileName}";
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Travel.BLL.DTOs.Bookings;
using Travel.Web.Helpers;
using Travel.Web.Resources;
using Travel.Web.Services.ApiClient;
using Travel.Web.ViewModels.Bookings;

namespace Travel.Web.Controllers;

[Authorize]
public class BookingsController : Controller
{
    private readonly ITravelApiClient _apiClient;
    private readonly IStringLocalizer<SharedResource> _localizer;

    public BookingsController(ITravelApiClient apiClient, IStringLocalizer<SharedResource> localizer)
    {
        _apiClient = apiClient;
        _localizer = localizer;
    }

    public async Task<IActionResult> Index()
    {
        var packageBookings = await _apiClient.GetMyBookingsAsync();
        var hotelBookings = await _apiClient.GetMyHotelBookingsAsync();

        string? whatsAppNumber = null;
        try
        {
            whatsAppNumber = (await _apiClient.GetSiteSettingsAsync()).WhatsAppNumber;
        }
        catch (ApiException)
        {
            // The bookings list still renders without the WhatsApp
            // buttons if settings can't be loaded for any reason.
        }

        return View(new MyBookingsViewModel
        {
            PackageBookings = packageBookings,
            HotelBookings = hotelBookings,
            WhatsAppNumber = whatsAppNumber
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateBookingDto dto)
    {
        if (!ModelState.IsValid)
        {
            TempData["Error"] = _localizer["BookingValidationError"].Value;
            return RedirectToAction("Details", "Packages", new { id = dto.TravelPackageId });
        }

        try
        {
            var booking = await _apiClient.CreateBookingAsync(dto);
            TempData["Success"] = _localizer["BookingCreated"].Value;

            try
            {
                var settings = await _apiClient.GetSiteSettingsAsync();
                var message = string.Format(_localizer["WhatsAppBookingMessage"].Value, booking.PackageTitle, booking.Id);
                TempData["WhatsAppUrl"] = WhatsAppHelper.BuildChatUrl(settings.WhatsAppNumber, message);
            }
            catch (ApiException)
            {
                // Booking already succeeded; the WhatsApp CTA is a bonus,
                // not something worth failing the whole request over.
            }

            return RedirectToAction(nameof(Index));
        }
        catch (ApiException ex)
        {
            TempData["Error"] = ex.Message;
            return RedirectToAction("Details", "Packages", new { id = dto.TravelPackageId });
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Cancel(int id)
    {
        try
        {
            await _apiClient.CancelBookingAsync(id);
            TempData["Success"] = _localizer["BookingCancelled"].Value;
        }
        catch (ApiException ex)
        {
            TempData["Error"] = ex.Message;
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CancelHotel(int id)
    {
        try
        {
            await _apiClient.CancelHotelBookingAsync(id);
            TempData["Success"] = _localizer["BookingCancelled"].Value;
        }
        catch (ApiException ex)
        {
            TempData["Error"] = ex.Message;
        }

        return RedirectToAction(nameof(Index));
    }
}

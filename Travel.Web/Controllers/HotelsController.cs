using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Travel.BLL.DTOs.Hotels;
using Travel.Web.Helpers;
using Travel.Web.Resources;
using Travel.Web.Services.ApiClient;
using Travel.Web.ViewModels.Hotels;

namespace Travel.Web.Controllers;

public class HotelsController : Controller
{
    private readonly ITravelApiClient _apiClient;
    private readonly IStringLocalizer<SharedResource> _localizer;

    public HotelsController(ITravelApiClient apiClient, IStringLocalizer<SharedResource> localizer)
    {
        _apiClient = apiClient;
        _localizer = localizer;
    }

    public async Task<IActionResult> Index([FromQuery] HotelSearchParamsDto searchParams)
    {
        var result = await _apiClient.SearchHotelsAsync(searchParams);
        var destinations = await _apiClient.GetDestinationsAsync();

        return View(new HotelListViewModel
        {
            Result = result,
            Destinations = destinations,
            SearchParams = searchParams
        });
    }

    public async Task<IActionResult> Details(int id)
    {
        try
        {
            var hotel = await _apiClient.GetHotelAsync(id);

            try
            {
                ViewBag.WhatsAppNumber = (await _apiClient.GetSiteSettingsAsync()).WhatsAppNumber;
            }
            catch (ApiException)
            {
                ViewBag.WhatsAppNumber = null;
            }

            return View(hotel);
        }
        catch (ApiException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return NotFound();
        }
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Reserve(CreateHotelBookingDto dto)
    {
        if (!ModelState.IsValid)
        {
            TempData["Error"] = _localizer["BookingValidationError"].Value;
            return RedirectToAction(nameof(Details), new { id = dto.HotelId });
        }

        try
        {
            var booking = await _apiClient.CreateHotelBookingAsync(dto);
            TempData["Success"] = _localizer["BookingCreated"].Value;

            try
            {
                var settings = await _apiClient.GetSiteSettingsAsync();
                var message = string.Format(_localizer["WhatsAppBookingMessage"].Value, booking.HotelName, booking.Id);
                TempData["WhatsAppUrl"] = WhatsAppHelper.BuildChatUrl(settings.WhatsAppNumber, message);
            }
            catch (ApiException)
            {
                // Reservation already succeeded; the WhatsApp CTA is a
                // bonus, not something worth failing the whole request over.
            }

            return RedirectToAction("Index", "Bookings");
        }
        catch (ApiException ex)
        {
            TempData["Error"] = ex.Message;
            return RedirectToAction(nameof(Details), new { id = dto.HotelId });
        }
    }
}

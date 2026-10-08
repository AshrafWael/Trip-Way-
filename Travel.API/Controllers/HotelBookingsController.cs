using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Travel.API.Extensions;
using Travel.BLL.DTOs.Common;
using Travel.BLL.DTOs.Hotels;
using Travel.BLL.Interfaces;

namespace Travel.API.Controllers;

[ApiController]
[Route("api/hotel-bookings")]
[Authorize]
public class HotelBookingsController : ControllerBase
{
    private readonly IHotelBookingService _hotelBookingService;

    public HotelBookingsController(IHotelBookingService hotelBookingService)
    {
        _hotelBookingService = hotelBookingService;
    }

    [HttpGet("my")]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<HotelBookingDto>>>> GetMyBookings(CancellationToken cancellationToken)
    {
        var bookings = await _hotelBookingService.GetForUserAsync(User.GetUserId(), cancellationToken);
        return Ok(ApiResponse<IReadOnlyList<HotelBookingDto>>.Ok(bookings));
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<HotelBookingDto>>> Create(CreateHotelBookingDto dto, CancellationToken cancellationToken)
    {
        var created = await _hotelBookingService.CreateAsync(User.GetUserId(), dto, cancellationToken);
        return Ok(ApiResponse<HotelBookingDto>.Ok(created));
    }

    [HttpPut("{id:int}/cancel")]
    public async Task<IActionResult> Cancel(int id, CancellationToken cancellationToken)
    {
        await _hotelBookingService.CancelAsync(id, User.GetUserId(), cancellationToken);
        return NoContent();
    }
}

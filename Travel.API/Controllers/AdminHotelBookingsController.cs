using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Travel.BLL.DTOs.Bookings;
using Travel.BLL.DTOs.Common;
using Travel.BLL.DTOs.Hotels;
using Travel.BLL.Interfaces;

namespace Travel.API.Controllers;

[ApiController]
[Route("api/admin/hotel-bookings")]
[Authorize(Roles = "Admin")]
public class AdminHotelBookingsController : ControllerBase
{
    private readonly IHotelBookingService _hotelBookingService;

    public AdminHotelBookingsController(IHotelBookingService hotelBookingService)
    {
        _hotelBookingService = hotelBookingService;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<HotelBookingDto>>>> GetAll(CancellationToken cancellationToken)
    {
        var bookings = await _hotelBookingService.GetAllAsync(cancellationToken);
        return Ok(ApiResponse<IReadOnlyList<HotelBookingDto>>.Ok(bookings));
    }

    [HttpPut("{id:int}/status")]
    public async Task<IActionResult> UpdateStatus(int id, UpdateBookingStatusDto dto, CancellationToken cancellationToken)
    {
        await _hotelBookingService.UpdateStatusAsync(id, dto.Status, cancellationToken);
        return NoContent();
    }
}

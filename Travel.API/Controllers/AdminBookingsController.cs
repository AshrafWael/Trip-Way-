using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Travel.BLL.DTOs.Bookings;
using Travel.BLL.DTOs.Common;
using Travel.BLL.Interfaces;

namespace Travel.API.Controllers;

[ApiController]
[Route("api/admin/bookings")]
[Authorize(Roles = "Admin")]
public class AdminBookingsController : ControllerBase
{
    private readonly IBookingService _bookingService;

    public AdminBookingsController(IBookingService bookingService)
    {
        _bookingService = bookingService;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<BookingDto>>>> GetAll(CancellationToken cancellationToken)
    {
        var bookings = await _bookingService.GetAllAsync(cancellationToken);
        return Ok(ApiResponse<IReadOnlyList<BookingDto>>.Ok(bookings));
    }

    [HttpPut("{id:int}/status")]
    public async Task<IActionResult> UpdateStatus(int id, UpdateBookingStatusDto dto, CancellationToken cancellationToken)
    {
        await _bookingService.UpdateStatusAsync(id, dto.Status, cancellationToken);
        return NoContent();
    }
}

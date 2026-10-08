using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Travel.API.Extensions;
using Travel.BLL.DTOs.Bookings;
using Travel.BLL.DTOs.Common;
using Travel.BLL.Interfaces;

namespace Travel.API.Controllers;

[ApiController]
[Route("api/bookings")]
[Authorize]
public class BookingsController : ControllerBase
{
    private readonly IBookingService _bookingService;

    public BookingsController(IBookingService bookingService)
    {
        _bookingService = bookingService;
    }

    [HttpGet("my")]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<BookingDto>>>> GetMyBookings(CancellationToken cancellationToken)
    {
        var bookings = await _bookingService.GetForUserAsync(User.GetUserId(), cancellationToken);
        return Ok(ApiResponse<IReadOnlyList<BookingDto>>.Ok(bookings));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ApiResponse<BookingDto>>> GetById(int id, CancellationToken cancellationToken)
    {
        var isAdmin = User.IsInRole("Admin");
        var booking = await _bookingService.GetByIdAsync(id, User.GetUserId(), isAdmin, cancellationToken);
        return Ok(ApiResponse<BookingDto>.Ok(booking));
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<BookingDto>>> Create(CreateBookingDto dto, CancellationToken cancellationToken)
    {
        var created = await _bookingService.CreateAsync(User.GetUserId(), dto, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, ApiResponse<BookingDto>.Ok(created));
    }

    [HttpPut("{id:int}/cancel")]
    public async Task<IActionResult> Cancel(int id, CancellationToken cancellationToken)
    {
        await _bookingService.CancelAsync(id, User.GetUserId(), cancellationToken);
        return NoContent();
    }
}

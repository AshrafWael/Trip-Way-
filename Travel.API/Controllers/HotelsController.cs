using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Travel.BLL.DTOs.Common;
using Travel.BLL.DTOs.Hotels;
using Travel.BLL.Interfaces;

namespace Travel.API.Controllers;

[ApiController]
[Route("api/hotels")]
public class HotelsController : ControllerBase
{
    private readonly IHotelService _hotelService;

    public HotelsController(IHotelService hotelService)
    {
        _hotelService = hotelService;
    }

    /// <summary>
    /// GET /api/hotels?pageNumber=1&amp;pageSize=12&amp;destinationId=5&amp;minPrice=1000&amp;maxPrice=5000&amp;minStarRating=4&amp;search=cairo&amp;sortBy=price_asc
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResultDto<HotelDto>>>> Search(
        [FromQuery] HotelSearchParamsDto searchParams, CancellationToken cancellationToken)
    {
        var result = await _hotelService.SearchAsync(searchParams, cancellationToken);
        return Ok(ApiResponse<PagedResultDto<HotelDto>>.Ok(result));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ApiResponse<HotelDetailsDto>>> GetById(int id, CancellationToken cancellationToken)
    {
        var hotel = await _hotelService.GetByIdAsync(id, cancellationToken);
        return Ok(ApiResponse<HotelDetailsDto>.Ok(hotel));
    }

    [HttpGet("admin/all")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<HotelDto>>>> GetAllForAdmin(CancellationToken cancellationToken)
    {
        var hotels = await _hotelService.GetAllForAdminAsync(cancellationToken);
        return Ok(ApiResponse<IReadOnlyList<HotelDto>>.Ok(hotels));
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<HotelDto>>> Create(CreateHotelDto dto, CancellationToken cancellationToken)
    {
        var created = await _hotelService.CreateAsync(dto, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, ApiResponse<HotelDto>.Ok(created));
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int id, UpdateHotelDto dto, CancellationToken cancellationToken)
    {
        await _hotelService.UpdateAsync(id, dto, cancellationToken);
        return NoContent();
    }

    [HttpPut("{id:int}/images")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateImages(int id, [FromBody] List<string> imageUrls, CancellationToken cancellationToken)
    {
        await _hotelService.UpdateImagesAsync(id, imageUrls, cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        await _hotelService.DeleteAsync(id, cancellationToken);
        return NoContent();
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Travel.BLL.DTOs.Common;
using Travel.BLL.DTOs.Destinations;
using Travel.BLL.Interfaces;

namespace Travel.API.Controllers;

[ApiController]
[Route("api/destinations")]
public class DestinationsController : ControllerBase
{
    private readonly IDestinationService _destinationService;

    public DestinationsController(IDestinationService destinationService)
    {
        _destinationService = destinationService;
    }

    /// <summary>
    /// includeInactive is for the admin management screen; inactive
    /// destinations aren't sensitive, just hidden from the public site,
    /// so this stays anonymous rather than adding an extra round trip.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<DestinationDto>>>> GetAll(
        [FromQuery] bool includeInactive, CancellationToken cancellationToken)
    {
        var destinations = await _destinationService.GetAllAsync(onlyActive: !includeInactive, cancellationToken);
        return Ok(ApiResponse<IReadOnlyList<DestinationDto>>.Ok(destinations));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ApiResponse<DestinationDto>>> GetById(int id, CancellationToken cancellationToken)
    {
        var destination = await _destinationService.GetByIdAsync(id, cancellationToken);
        return Ok(ApiResponse<DestinationDto>.Ok(destination));
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<DestinationDto>>> Create(CreateDestinationDto dto, CancellationToken cancellationToken)
    {
        var created = await _destinationService.CreateAsync(dto, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, ApiResponse<DestinationDto>.Ok(created));
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int id, UpdateDestinationDto dto, CancellationToken cancellationToken)
    {
        await _destinationService.UpdateAsync(id, dto, cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        await _destinationService.DeleteAsync(id, cancellationToken);
        return NoContent();
    }
}

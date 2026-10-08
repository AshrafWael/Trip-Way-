using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Travel.BLL.DTOs.Common;
using Travel.BLL.DTOs.Packages;
using Travel.BLL.Interfaces;

namespace Travel.API.Controllers;

[ApiController]
[Route("api/packages")]
public class PackagesController : ControllerBase
{
    private readonly IPackageService _packageService;

    public PackagesController(IPackageService packageService)
    {
        _packageService = packageService;
    }

    /// <summary>
    /// GET /api/packages?pageNumber=1&amp;pageSize=12&amp;destinationId=5&amp;minPrice=1000&amp;maxPrice=5000&amp;search=cairo&amp;sortBy=price_asc&amp;travelers=2
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResultDto<PackageDto>>>> Search(
        [FromQuery] PackageSearchParamsDto searchParams, CancellationToken cancellationToken)
    {
        var result = await _packageService.SearchAsync(searchParams, cancellationToken);
        return Ok(ApiResponse<PagedResultDto<PackageDto>>.Ok(result));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ApiResponse<PackageDetailsDto>>> GetById(int id, CancellationToken cancellationToken)
    {
        var package = await _packageService.GetByIdAsync(id, cancellationToken);
        return Ok(ApiResponse<PackageDetailsDto>.Ok(package));
    }

    [HttpGet("admin/all")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<PackageDto>>>> GetAllForAdmin(CancellationToken cancellationToken)
    {
        var packages = await _packageService.GetAllForAdminAsync(cancellationToken);
        return Ok(ApiResponse<IReadOnlyList<PackageDto>>.Ok(packages));
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<PackageDto>>> Create(CreatePackageDto dto, CancellationToken cancellationToken)
    {
        var created = await _packageService.CreateAsync(dto, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, ApiResponse<PackageDto>.Ok(created));
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int id, UpdatePackageDto dto, CancellationToken cancellationToken)
    {
        await _packageService.UpdateAsync(id, dto, cancellationToken);
        return NoContent();
    }

    [HttpPut("{id:int}/images")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateImages(int id, [FromBody] List<string> imageUrls, CancellationToken cancellationToken)
    {
        await _packageService.UpdateImagesAsync(id, imageUrls, cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        await _packageService.DeleteAsync(id, cancellationToken);
        return NoContent();
    }
}

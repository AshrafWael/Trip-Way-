using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Travel.BLL.DTOs.Common;
using Travel.BLL.DTOs.Settings;
using Travel.BLL.Interfaces;

namespace Travel.API.Controllers;

[ApiController]
[Route("api/settings")]
public class SettingsController : ControllerBase
{
    private readonly ISiteSettingsService _siteSettingsService;

    public SettingsController(ISiteSettingsService siteSettingsService)
    {
        _siteSettingsService = siteSettingsService;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<SiteSettingsDto>>> Get(CancellationToken cancellationToken)
    {
        var settings = await _siteSettingsService.GetAsync(cancellationToken);
        return Ok(ApiResponse<SiteSettingsDto>.Ok(settings));
    }

    [HttpPut]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(UpdateSiteSettingsDto dto, CancellationToken cancellationToken)
    {
        await _siteSettingsService.UpdateAsync(dto, cancellationToken);
        return NoContent();
    }
}

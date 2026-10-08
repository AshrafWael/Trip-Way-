using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Travel.BLL.DTOs.Admin;
using Travel.BLL.DTOs.Common;
using Travel.BLL.Interfaces;

namespace Travel.API.Controllers;

[ApiController]
[Route("api/admin/dashboard")]
[Authorize(Roles = "Admin")]
public class AdminDashboardController : ControllerBase
{
    private readonly IDashboardService _dashboardService;

    public AdminDashboardController(IDashboardService dashboardService)
    {
        _dashboardService = dashboardService;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<DashboardStatsDto>>> GetStats(CancellationToken cancellationToken)
    {
        var stats = await _dashboardService.GetStatsAsync(cancellationToken);
        return Ok(ApiResponse<DashboardStatsDto>.Ok(stats));
    }
}

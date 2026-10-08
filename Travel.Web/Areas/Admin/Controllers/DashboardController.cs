using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Travel.Web.Services.ApiClient;

namespace Travel.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class DashboardController : Controller
{
    private readonly ITravelApiClient _apiClient;

    public DashboardController(ITravelApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public async Task<IActionResult> Index()
    {
        var stats = await _apiClient.GetDashboardStatsAsync();
        return View(stats);
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Travel.BLL.DTOs.Admin;
using Travel.BLL.DTOs.Common;
using Travel.BLL.Interfaces;

namespace Travel.API.Controllers;

[ApiController]
[Route("api/admin/users")]
[Authorize(Roles = "Admin")]
public class AdminUsersController : ControllerBase
{
    private readonly IAdminUserService _adminUserService;

    public AdminUsersController(IAdminUserService adminUserService)
    {
        _adminUserService = adminUserService;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<UserSummaryDto>>>> GetAll(CancellationToken cancellationToken)
    {
        var users = await _adminUserService.GetAllAsync(cancellationToken);
        return Ok(ApiResponse<IReadOnlyList<UserSummaryDto>>.Ok(users));
    }

    [HttpPut("{id}/lock")]
    public async Task<IActionResult> Lock(string id, CancellationToken cancellationToken)
    {
        await _adminUserService.SetLockedAsync(id, true, cancellationToken);
        return NoContent();
    }

    [HttpPut("{id}/unlock")]
    public async Task<IActionResult> Unlock(string id, CancellationToken cancellationToken)
    {
        await _adminUserService.SetLockedAsync(id, false, cancellationToken);
        return NoContent();
    }
}

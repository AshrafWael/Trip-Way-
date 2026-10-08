using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Travel.BLL.DTOs.Common;
using Travel.BLL.DTOs.Reviews;
using Travel.BLL.Interfaces;

namespace Travel.API.Controllers;

[ApiController]
[Route("api/admin/reviews")]
[Authorize(Roles = "Admin")]
public class AdminReviewsController : ControllerBase
{
    private readonly IReviewService _reviewService;

    public AdminReviewsController(IReviewService reviewService)
    {
        _reviewService = reviewService;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<ReviewDto>>>> GetAll(CancellationToken cancellationToken)
    {
        var reviews = await _reviewService.GetAllForAdminAsync(cancellationToken);
        return Ok(ApiResponse<IReadOnlyList<ReviewDto>>.Ok(reviews));
    }

    [HttpPut("{id:int}/approve")]
    public async Task<IActionResult> Approve(int id, CancellationToken cancellationToken)
    {
        await _reviewService.ApproveAsync(id, cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        await _reviewService.DeleteAsync(id, cancellationToken);
        return NoContent();
    }
}

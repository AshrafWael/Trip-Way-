using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Travel.API.Extensions;
using Travel.BLL.DTOs.Common;
using Travel.BLL.DTOs.Reviews;
using Travel.BLL.Interfaces;

namespace Travel.API.Controllers;

[ApiController]
[Route("api/packages/{packageId:int}/reviews")]
public class ReviewsController : ControllerBase
{
    private readonly IReviewService _reviewService;

    public ReviewsController(IReviewService reviewService)
    {
        _reviewService = reviewService;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<ReviewDto>>>> GetForPackage(int packageId, CancellationToken cancellationToken)
    {
        var reviews = await _reviewService.GetForPackageAsync(packageId, cancellationToken);
        return Ok(ApiResponse<IReadOnlyList<ReviewDto>>.Ok(reviews));
    }

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<ApiResponse<ReviewDto>>> Create(int packageId, CreateReviewDto dto, CancellationToken cancellationToken)
    {
        // The package id in the route is authoritative; keep the body in sync
        // rather than trusting a possibly different value from the client.
        dto.TravelPackageId = packageId;

        var created = await _reviewService.CreateAsync(User.GetUserId(), dto, cancellationToken);
        return CreatedAtAction(nameof(GetForPackage), new { packageId }, ApiResponse<ReviewDto>.Ok(created));
    }
}

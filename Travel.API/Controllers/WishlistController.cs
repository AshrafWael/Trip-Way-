using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Travel.API.Extensions;
using Travel.BLL.DTOs.Common;
using Travel.BLL.DTOs.Wishlist;
using Travel.BLL.Interfaces;

namespace Travel.API.Controllers;

[ApiController]
[Route("api/wishlist")]
[Authorize]
public class WishlistController : ControllerBase
{
    private readonly IWishlistService _wishlistService;

    public WishlistController(IWishlistService wishlistService)
    {
        _wishlistService = wishlistService;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<WishlistItemDto>>>> GetMyWishlist(CancellationToken cancellationToken)
    {
        var items = await _wishlistService.GetForUserAsync(User.GetUserId(), cancellationToken);
        return Ok(ApiResponse<IReadOnlyList<WishlistItemDto>>.Ok(items));
    }

    [HttpPost("{packageId:int}")]
    public async Task<IActionResult> Add(int packageId, CancellationToken cancellationToken)
    {
        await _wishlistService.AddAsync(User.GetUserId(), packageId, cancellationToken);
        return NoContent();
    }

    [HttpDelete("{packageId:int}")]
    public async Task<IActionResult> Remove(int packageId, CancellationToken cancellationToken)
    {
        await _wishlistService.RemoveAsync(User.GetUserId(), packageId, cancellationToken);
        return NoContent();
    }
}

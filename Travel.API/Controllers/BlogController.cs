using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Travel.API.Extensions;
using Travel.BLL.DTOs.Blog;
using Travel.BLL.DTOs.Common;
using Travel.BLL.Interfaces;

namespace Travel.API.Controllers;

[ApiController]
[Route("api/blog")]
public class BlogController : ControllerBase
{
    private readonly IBlogService _blogService;

    public BlogController(IBlogService blogService)
    {
        _blogService = blogService;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<BlogPostDto>>>> GetPublished(CancellationToken cancellationToken)
    {
        var posts = await _blogService.GetPublishedAsync(cancellationToken);
        return Ok(ApiResponse<IReadOnlyList<BlogPostDto>>.Ok(posts));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ApiResponse<BlogPostDto>>> GetById(int id, CancellationToken cancellationToken)
    {
        var post = await _blogService.GetByIdAsync(id, cancellationToken);
        return Ok(ApiResponse<BlogPostDto>.Ok(post));
    }

    [HttpGet("admin/all")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<BlogPostDto>>>> GetAllForAdmin(CancellationToken cancellationToken)
    {
        var posts = await _blogService.GetAllAsync(cancellationToken);
        return Ok(ApiResponse<IReadOnlyList<BlogPostDto>>.Ok(posts));
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<BlogPostDto>>> Create(CreateBlogPostDto dto, CancellationToken cancellationToken)
    {
        var created = await _blogService.CreateAsync(User.GetUserId(), dto, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, ApiResponse<BlogPostDto>.Ok(created));
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int id, UpdateBlogPostDto dto, CancellationToken cancellationToken)
    {
        await _blogService.UpdateAsync(id, dto, cancellationToken);
        return NoContent();
    }

    [HttpPut("{id:int}/images")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateImages(int id, [FromBody] List<string> imageUrls, CancellationToken cancellationToken)
    {
        await _blogService.UpdateImagesAsync(id, imageUrls, cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        await _blogService.DeleteAsync(id, cancellationToken);
        return NoContent();
    }
}

using Travel.BLL.DTOs.Blog;

namespace Travel.BLL.Interfaces;

public interface IBlogService
{
    Task<IReadOnlyList<BlogPostDto>> GetPublishedAsync(CancellationToken cancellationToken = default);
    Task<BlogPostDto> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    // Admin
    Task<IReadOnlyList<BlogPostDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<BlogPostDto> CreateAsync(string authorId, CreateBlogPostDto dto, CancellationToken cancellationToken = default);
    Task UpdateAsync(int id, UpdateBlogPostDto dto, CancellationToken cancellationToken = default);
    Task UpdateImagesAsync(int id, IReadOnlyList<string> imageUrls, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}

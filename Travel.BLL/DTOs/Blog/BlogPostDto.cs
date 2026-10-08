namespace Travel.BLL.DTOs.Blog;

public class BlogPostDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string ArabicTitle { get; set; } = string.Empty;
    public string? Subtitle { get; set; }
    public string? ArabicSubtitle { get; set; }
    public string Content { get; set; } = string.Empty;
    public string ArabicContent { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public List<BlogPostImageDto> Images { get; set; } = new();
    public string AuthorName { get; set; } = string.Empty;
    public DateTime? PublishedAt { get; set; }
    public bool IsPublished { get; set; }
}

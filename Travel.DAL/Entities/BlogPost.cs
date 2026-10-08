namespace Travel.DAL.Entities;

public class BlogPost
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;
    public string ArabicTitle { get; set; } = string.Empty;

    public string? Subtitle { get; set; }
    public string? ArabicSubtitle { get; set; }

    public string Content { get; set; } = string.Empty;
    public string ArabicContent { get; set; } = string.Empty;

    public string? ImageUrl { get; set; }

    public string AuthorId { get; set; } = string.Empty;
    public ApplicationUser Author { get; set; } = null!;

    public DateTime? PublishedAt { get; set; }
    public bool IsPublished { get; set; }

    public ICollection<BlogPostImage> Images { get; set; } = new List<BlogPostImage>();
}

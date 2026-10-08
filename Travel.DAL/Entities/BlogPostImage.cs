namespace Travel.DAL.Entities;

public class BlogPostImage
{
    public int Id { get; set; }

    public int BlogPostId { get; set; }
    public BlogPost BlogPost { get; set; } = null!;

    public string ImageUrl { get; set; } = string.Empty;
    public int DisplayOrder { get; set; }
}

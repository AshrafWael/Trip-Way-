using System.ComponentModel.DataAnnotations;

namespace Travel.BLL.DTOs.Blog;

public class CreateBlogPostDto
{
    [Required, MaxLength(250), Display(Name = "Field_Title")]
    public string Title { get; set; } = string.Empty;

    [Required, MaxLength(250), Display(Name = "ArabicTitle")]
    public string ArabicTitle { get; set; } = string.Empty;

    [MaxLength(400), Display(Name = "Subtitle")]
    public string? Subtitle { get; set; }

    [MaxLength(400), Display(Name = "ArabicSubtitle")]
    public string? ArabicSubtitle { get; set; }

    [Required, Display(Name = "Content")]
    public string Content { get; set; } = string.Empty;

    [Required, Display(Name = "ArabicContent")]
    public string ArabicContent { get; set; } = string.Empty;

    [MaxLength(500), Display(Name = "ImageUrl")]
    public string? ImageUrl { get; set; }

    [Display(Name = "IsPublished")]
    public bool IsPublished { get; set; }

    public List<string> ImageUrls { get; set; } = new();
}

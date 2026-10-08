using System.ComponentModel.DataAnnotations;

namespace Travel.BLL.DTOs.Reviews;

public class CreateReviewDto
{
    [Required]
    public int TravelPackageId { get; set; }

    [Range(1, 5), Display(Name = "Rating")]
    public int Rating { get; set; }

    [MaxLength(2000), Display(Name = "Comment")]
    public string? Comment { get; set; }
}

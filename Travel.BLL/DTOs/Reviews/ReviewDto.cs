namespace Travel.BLL.DTOs.Reviews;

public class ReviewDto
{
    public int Id { get; set; }
    public int TravelPackageId { get; set; }
    public string PackageTitle { get; set; } = string.Empty;
    public string UserFullName { get; set; } = string.Empty;
    public int Rating { get; set; }
    public string? Comment { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsApproved { get; set; }
}

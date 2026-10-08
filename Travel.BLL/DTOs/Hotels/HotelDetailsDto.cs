namespace Travel.BLL.DTOs.Hotels;

/// <summary>Full shape used on the hotel details page.</summary>
public class HotelDetailsDto : HotelDto
{
    public string Description { get; set; } = string.Empty;
    public string ArabicDescription { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string ArabicAddress { get; set; } = string.Empty;
    public List<HotelImageDto> Images { get; set; } = new();
}

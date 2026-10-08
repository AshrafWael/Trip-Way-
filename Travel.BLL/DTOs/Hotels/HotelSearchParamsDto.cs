namespace Travel.BLL.DTOs.Hotels;

/// <summary>Bound from the GET /api/hotels query string.</summary>
public class HotelSearchParamsDto
{
    private const int MaxPageSize = 50;
    private int _pageSize = 12;

    public int PageNumber { get; set; } = 1;

    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = value is > 0 and <= MaxPageSize ? value : MaxPageSize;
    }

    public int? DestinationId { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public int? MinStarRating { get; set; }
    public string? Search { get; set; }

    /// <summary>One of: price_asc, price_desc, newest, featured (default).</summary>
    public string? SortBy { get; set; }
}

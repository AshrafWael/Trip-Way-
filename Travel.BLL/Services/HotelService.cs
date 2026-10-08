using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Travel.BLL.DTOs.Common;
using Travel.BLL.DTOs.Hotels;
using Travel.BLL.Exceptions;
using Travel.BLL.Interfaces;
using Travel.DAL.Entities;
using Travel.DAL.UnitOfWork;

namespace Travel.BLL.Services;

public class HotelService : IHotelService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public HotelService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<PagedResultDto<HotelDto>> SearchAsync(HotelSearchParamsDto searchParams, CancellationToken cancellationToken = default)
    {
        // Hotels don't need the bespoke repository Packages has (no
        // seats-based filter, no reviews join) so the filtering lives
        // directly here against the generic repository's IQueryable.
        var query = _unitOfWork.Hotels.Query()
            .Include(h => h.Destination)
            .Where(h => h.IsActive);

        if (searchParams.DestinationId.HasValue)
            query = query.Where(h => h.DestinationId == searchParams.DestinationId.Value);

        if (searchParams.MinPrice.HasValue)
            query = query.Where(h => (h.DiscountPricePerNight ?? h.PricePerNight) >= searchParams.MinPrice.Value);

        if (searchParams.MaxPrice.HasValue)
            query = query.Where(h => (h.DiscountPricePerNight ?? h.PricePerNight) <= searchParams.MaxPrice.Value);

        if (searchParams.MinStarRating.HasValue)
            query = query.Where(h => h.StarRating >= searchParams.MinStarRating.Value);

        if (!string.IsNullOrWhiteSpace(searchParams.Search))
        {
            var term = searchParams.Search.Trim();
            query = query.Where(h =>
                h.Name.Contains(term) || h.ArabicName.Contains(term) ||
                h.Destination.Name.Contains(term) || h.Destination.ArabicName.Contains(term));
        }

        query = searchParams.SortBy switch
        {
            "price_asc" => query.OrderBy(h => h.DiscountPricePerNight ?? h.PricePerNight),
            "price_desc" => query.OrderByDescending(h => h.DiscountPricePerNight ?? h.PricePerNight),
            "newest" => query.OrderByDescending(h => h.CreatedAt),
            _ => query.OrderByDescending(h => h.IsFeatured).ThenByDescending(h => h.CreatedAt)
        };

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .Skip((searchParams.PageNumber - 1) * searchParams.PageSize)
            .Take(searchParams.PageSize)
            .ToListAsync(cancellationToken);

        return new PagedResultDto<HotelDto>
        {
            Items = _mapper.Map<IReadOnlyList<HotelDto>>(items),
            PageNumber = searchParams.PageNumber,
            PageSize = searchParams.PageSize,
            TotalCount = totalCount
        };
    }

    public async Task<HotelDetailsDto> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var hotel = await _unitOfWork.Hotels.Query()
            .Include(h => h.Destination)
            .Include(h => h.Images)
            .FirstOrDefaultAsync(h => h.Id == id, cancellationToken)
            ?? throw new NotFoundException(nameof(Hotel), id);

        return _mapper.Map<HotelDetailsDto>(hotel);
    }

    public async Task<HotelDto> CreateAsync(CreateHotelDto dto, CancellationToken cancellationToken = default)
    {
        var destinationExists = await _unitOfWork.Destinations.AnyAsync(d => d.Id == dto.DestinationId, cancellationToken);
        if (!destinationExists)
            throw new BusinessRuleException($"Destination with id '{dto.DestinationId}' does not exist.");

        var hotel = _mapper.Map<Hotel>(dto);

        var order = 0;
        foreach (var url in dto.ImageUrls)
            hotel.Images.Add(new HotelImage { ImageUrl = url, DisplayOrder = order++ });

        await _unitOfWork.Hotels.AddAsync(hotel, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var created = await _unitOfWork.Hotels.Query()
            .Include(h => h.Destination)
            .FirstAsync(h => h.Id == hotel.Id, cancellationToken);
        return _mapper.Map<HotelDto>(created);
    }

    public async Task UpdateAsync(int id, UpdateHotelDto dto, CancellationToken cancellationToken = default)
    {
        var hotel = await _unitOfWork.Hotels.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException(nameof(Hotel), id);

        _mapper.Map(dto, hotel);
        hotel.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.Hotels.Update(hotel);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<HotelDto>> GetAllForAdminAsync(CancellationToken cancellationToken = default)
    {
        var hotels = await _unitOfWork.Hotels.Query()
            .Include(h => h.Destination)
            .OrderByDescending(h => h.CreatedAt)
            .ToListAsync(cancellationToken);

        return _mapper.Map<IReadOnlyList<HotelDto>>(hotels);
    }

    public async Task UpdateImagesAsync(int id, IReadOnlyList<string> imageUrls, CancellationToken cancellationToken = default)
    {
        var hotel = await _unitOfWork.Hotels.Query(asNoTracking: false)
            .Include(h => h.Images)
            .FirstOrDefaultAsync(h => h.Id == id, cancellationToken)
            ?? throw new NotFoundException(nameof(Hotel), id);

        foreach (var existingImage in hotel.Images.ToList())
            _unitOfWork.HotelImages.Remove(existingImage);

        var order = 0;
        foreach (var url in imageUrls.Where(u => !string.IsNullOrWhiteSpace(u)))
        {
            hotel.Images.Add(new HotelImage { ImageUrl = url.Trim(), DisplayOrder = order++ });
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var hotel = await _unitOfWork.Hotels.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException(nameof(Hotel), id);

        // Soft delete -- bookings reference hotels for history.
        hotel.IsActive = false;
        _unitOfWork.Hotels.Update(hotel);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}

using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Travel.BLL.DTOs.Common;
using Travel.BLL.DTOs.Packages;
using Travel.BLL.Exceptions;
using Travel.BLL.Interfaces;
using Travel.DAL.Entities;
using Travel.DAL.UnitOfWork;

namespace Travel.BLL.Services;

public class PackageService : IPackageService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public PackageService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<PagedResultDto<PackageDto>> SearchAsync(PackageSearchParamsDto searchParams, CancellationToken cancellationToken = default)
    {
        var (items, totalCount) = await _unitOfWork.Packages.SearchAsync(
            searchParams.PageNumber,
            searchParams.PageSize,
            searchParams.DestinationId,
            searchParams.MinPrice,
            searchParams.MaxPrice,
            searchParams.Search,
            searchParams.SortBy,
            searchParams.Travelers,
            cancellationToken);

        return new PagedResultDto<PackageDto>
        {
            Items = _mapper.Map<IReadOnlyList<PackageDto>>(items),
            PageNumber = searchParams.PageNumber,
            PageSize = searchParams.PageSize,
            TotalCount = totalCount
        };
    }

    public async Task<PackageDetailsDto> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var package = await _unitOfWork.Packages.GetWithDetailsAsync(id, cancellationToken)
            ?? throw new NotFoundException(nameof(TravelPackage), id);

        return _mapper.Map<PackageDetailsDto>(package);
    }

    public async Task<PackageDto> CreateAsync(CreatePackageDto dto, CancellationToken cancellationToken = default)
    {
        var destinationExists = await _unitOfWork.Destinations.AnyAsync(d => d.Id == dto.DestinationId, cancellationToken);
        if (!destinationExists)
            throw new BusinessRuleException($"Destination with id '{dto.DestinationId}' does not exist.");

        var package = _mapper.Map<TravelPackage>(dto);

        var order = 0;
        foreach (var url in dto.ImageUrls)
            package.Images.Add(new PackageImage { ImageUrl = url, DisplayOrder = order++ });

        await _unitOfWork.Packages.AddAsync(package, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Re-fetch with Destination/Images/Reviews eagerly loaded so the
        // returned DTO is fully populated rather than relying on
        // in-memory navigation fixup.
        var created = await _unitOfWork.Packages.GetWithDetailsAsync(package.Id, cancellationToken);
        return _mapper.Map<PackageDto>(created);
    }

    public async Task UpdateAsync(int id, UpdatePackageDto dto, CancellationToken cancellationToken = default)
    {
        var package = await _unitOfWork.Packages.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException(nameof(TravelPackage), id);

        _mapper.Map(dto, package);
        package.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.Packages.Update(package);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<PackageDto>> GetAllForAdminAsync(CancellationToken cancellationToken = default)
    {
        var packages = await _unitOfWork.Packages.Query()
            .Include(p => p.Destination)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync(cancellationToken);

        return _mapper.Map<IReadOnlyList<PackageDto>>(packages);
    }

    public async Task UpdateImagesAsync(int id, IReadOnlyList<string> imageUrls, CancellationToken cancellationToken = default)
    {
        var package = await _unitOfWork.Packages.Query(asNoTracking: false)
            .Include(p => p.Images)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken)
            ?? throw new NotFoundException(nameof(TravelPackage), id);

        foreach (var existingImage in package.Images.ToList())
            _unitOfWork.PackageImages.Remove(existingImage);

        var order = 0;
        foreach (var url in imageUrls.Where(u => !string.IsNullOrWhiteSpace(u)))
        {
            package.Images.Add(new PackageImage { ImageUrl = url.Trim(), DisplayOrder = order++ });
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var package = await _unitOfWork.Packages.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException(nameof(TravelPackage), id);

        // Soft delete for the same reporting/history reason as
        // Destination -- bookings and reviews reference them for history.
        package.IsActive = false;
        _unitOfWork.Packages.Update(package);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}

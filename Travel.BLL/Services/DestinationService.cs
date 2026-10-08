using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Travel.BLL.DTOs.Destinations;
using Travel.BLL.Exceptions;
using Travel.BLL.Interfaces;
using Travel.DAL.Entities;
using Travel.DAL.UnitOfWork;

namespace Travel.BLL.Services;

public class DestinationService : IDestinationService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public DestinationService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<DestinationDto>> GetAllAsync(bool onlyActive = true, CancellationToken cancellationToken = default)
    {
        var query = _unitOfWork.Destinations.Query();
        if (onlyActive)
            query = query.Where(d => d.IsActive);

        var destinations = await query
            .OrderByDescending(d => d.IsFeatured)
            .ThenBy(d => d.Name)
            .ToListAsync(cancellationToken);

        return _mapper.Map<IReadOnlyList<DestinationDto>>(destinations);
    }

    public async Task<DestinationDto> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var destination = await _unitOfWork.Destinations.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException(nameof(Destination), id);

        return _mapper.Map<DestinationDto>(destination);
    }

    public async Task<DestinationDto> CreateAsync(CreateDestinationDto dto, CancellationToken cancellationToken = default)
    {
        var destination = _mapper.Map<Destination>(dto);
        await _unitOfWork.Destinations.AddAsync(destination, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<DestinationDto>(destination);
    }

    public async Task UpdateAsync(int id, UpdateDestinationDto dto, CancellationToken cancellationToken = default)
    {
        var destination = await _unitOfWork.Destinations.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException(nameof(Destination), id);

        _mapper.Map(dto, destination);
        _unitOfWork.Destinations.Update(destination);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var destination = await _unitOfWork.Destinations.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException(nameof(Destination), id);

        // Soft delete: a destination with historical packages/bookings
        // must stay queryable for reporting, so it's deactivated rather
        // than removed (the FK is DeleteBehavior.Restrict for the same
        // reason -- see TravelPackageConfiguration).
        destination.IsActive = false;
        _unitOfWork.Destinations.Update(destination);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}

using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Travel.BLL.DTOs.Bookings;
using Travel.BLL.Exceptions;
using Travel.BLL.Interfaces;
using Travel.DAL.Entities;
using Travel.DAL.UnitOfWork;

namespace Travel.BLL.Services;

public class BookingService : IBookingService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public BookingService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<BookingDto>> GetForUserAsync(string userId, CancellationToken cancellationToken = default)
    {
        var bookings = await _unitOfWork.Bookings.Query()
            .Include(b => b.TravelPackage)
            .Where(b => b.UserId == userId)
            .OrderByDescending(b => b.CreatedAt)
            .ToListAsync(cancellationToken);

        return _mapper.Map<IReadOnlyList<BookingDto>>(bookings);
    }

    public async Task<BookingDto> GetByIdAsync(int id, string userId, bool isAdmin, CancellationToken cancellationToken = default)
    {
        var booking = await _unitOfWork.Bookings.Query()
            .Include(b => b.TravelPackage)
            .FirstOrDefaultAsync(b => b.Id == id, cancellationToken)
            ?? throw new NotFoundException(nameof(Booking), id);

        if (!isAdmin && booking.UserId != userId)
            throw new BusinessRuleException("You do not have access to this booking.");

        return _mapper.Map<BookingDto>(booking);
    }

    public async Task<BookingDto> CreateAsync(string userId, CreateBookingDto dto, CancellationToken cancellationToken = default)
    {
        var package = await _unitOfWork.Packages.GetByIdAsync(dto.TravelPackageId, cancellationToken)
            ?? throw new NotFoundException(nameof(TravelPackage), dto.TravelPackageId);

        if (!package.IsActive)
            throw new BusinessRuleException("This package is no longer available.");

        if (dto.NumberOfTravelers > package.AvailableSeats)
            throw new BusinessRuleException(
                $"Only {package.AvailableSeats} seat(s) left for this package.");

        // Price is optional (final pricing for some trips is only settled
        // via the WhatsApp conversation with customer service), so a
        // missing price becomes a 0 placeholder rather than a crash --
        // staff can follow up with the traveler directly.
        var unitPrice = package.DiscountPrice ?? package.Price ?? 0m;

        var booking = new Booking
        {
            UserId = userId,
            TravelPackageId = package.Id,
            TravelDate = dto.TravelDate,
            NumberOfTravelers = dto.NumberOfTravelers,
            TotalPrice = unitPrice * dto.NumberOfTravelers,
            SpecialRequests = dto.SpecialRequests,
            Status = BookingStatus.Pending
        };

        // Seat count and the booking row must move together, so both
        // changes go through the same SaveChangesAsync via the UoW.
        package.AvailableSeats -= dto.NumberOfTravelers;
        _unitOfWork.Packages.Update(package);

        await _unitOfWork.Bookings.AddAsync(booking, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<BookingDto>(booking);
    }

    public async Task CancelAsync(int id, string userId, CancellationToken cancellationToken = default)
    {
        var booking = await _unitOfWork.Bookings.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException(nameof(Booking), id);

        if (booking.UserId != userId)
            throw new BusinessRuleException("You do not have access to this booking.");

        if (booking.Status is BookingStatus.Cancelled or BookingStatus.Completed)
            throw new BusinessRuleException($"A {booking.Status} booking cannot be cancelled.");

        booking.Status = BookingStatus.Cancelled;
        _unitOfWork.Bookings.Update(booking);

        var package = await _unitOfWork.Packages.GetByIdAsync(booking.TravelPackageId, cancellationToken);
        if (package is not null)
        {
            package.AvailableSeats += booking.NumberOfTravelers;
            _unitOfWork.Packages.Update(package);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<BookingDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var bookings = await _unitOfWork.Bookings.Query()
            .Include(b => b.TravelPackage)
            .OrderByDescending(b => b.CreatedAt)
            .ToListAsync(cancellationToken);

        return _mapper.Map<IReadOnlyList<BookingDto>>(bookings);
    }

    public async Task UpdateStatusAsync(int id, BookingStatus status, CancellationToken cancellationToken = default)
    {
        var booking = await _unitOfWork.Bookings.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException(nameof(Booking), id);

        booking.Status = status;
        _unitOfWork.Bookings.Update(booking);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}

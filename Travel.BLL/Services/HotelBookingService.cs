using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Travel.BLL.DTOs.Hotels;
using Travel.BLL.Exceptions;
using Travel.BLL.Interfaces;
using Travel.DAL.Entities;
using Travel.DAL.UnitOfWork;

namespace Travel.BLL.Services;

public class HotelBookingService : IHotelBookingService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public HotelBookingService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<HotelBookingDto>> GetForUserAsync(string userId, CancellationToken cancellationToken = default)
    {
        var bookings = await _unitOfWork.HotelBookings.Query()
            .Include(b => b.Hotel)
            .Where(b => b.UserId == userId)
            .OrderByDescending(b => b.CreatedAt)
            .ToListAsync(cancellationToken);

        return _mapper.Map<IReadOnlyList<HotelBookingDto>>(bookings);
    }

    public async Task<HotelBookingDto> CreateAsync(string userId, CreateHotelBookingDto dto, CancellationToken cancellationToken = default)
    {
        var hotel = await _unitOfWork.Hotels.GetByIdAsync(dto.HotelId, cancellationToken)
            ?? throw new NotFoundException(nameof(Hotel), dto.HotelId);

        if (!hotel.IsActive)
            throw new BusinessRuleException("This hotel is no longer available.");

        if (dto.CheckOutDate <= dto.CheckInDate)
            throw new BusinessRuleException("Check-out date must be after the check-in date.");

        if (dto.NumberOfRooms > hotel.AvailableRooms)
            throw new BusinessRuleException($"Only {hotel.AvailableRooms} room(s) left at this hotel.");

        var nights = (dto.CheckOutDate.Date - dto.CheckInDate.Date).Days;
        // Price is optional (some hotels are priced only after a WhatsApp
        // conversation with customer service), so a missing price becomes
        // a 0 placeholder rather than a crash.
        var unitPrice = hotel.DiscountPricePerNight ?? hotel.PricePerNight ?? 0m;

        var booking = new HotelBooking
        {
            UserId = userId,
            HotelId = hotel.Id,
            CheckInDate = dto.CheckInDate,
            CheckOutDate = dto.CheckOutDate,
            NumberOfRooms = dto.NumberOfRooms,
            NumberOfGuests = dto.NumberOfGuests,
            TotalPrice = unitPrice * dto.NumberOfRooms * nights,
            SpecialRequests = dto.SpecialRequests,
            Status = BookingStatus.Pending
        };

        // Room count and the booking row must move together, so both
        // changes go through the same SaveChangesAsync via the UoW.
        hotel.AvailableRooms -= dto.NumberOfRooms;
        _unitOfWork.Hotels.Update(hotel);

        await _unitOfWork.HotelBookings.AddAsync(booking, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<HotelBookingDto>(booking);
    }

    public async Task CancelAsync(int id, string userId, CancellationToken cancellationToken = default)
    {
        var booking = await _unitOfWork.HotelBookings.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException(nameof(HotelBooking), id);

        if (booking.UserId != userId)
            throw new BusinessRuleException("You do not have access to this booking.");

        if (booking.Status is BookingStatus.Cancelled or BookingStatus.Completed)
            throw new BusinessRuleException($"A {booking.Status} booking cannot be cancelled.");

        booking.Status = BookingStatus.Cancelled;
        _unitOfWork.HotelBookings.Update(booking);

        var hotel = await _unitOfWork.Hotels.GetByIdAsync(booking.HotelId, cancellationToken);
        if (hotel is not null)
        {
            hotel.AvailableRooms += booking.NumberOfRooms;
            _unitOfWork.Hotels.Update(hotel);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<HotelBookingDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var bookings = await _unitOfWork.HotelBookings.Query()
            .Include(b => b.Hotel)
            .OrderByDescending(b => b.CreatedAt)
            .ToListAsync(cancellationToken);

        return _mapper.Map<IReadOnlyList<HotelBookingDto>>(bookings);
    }

    public async Task UpdateStatusAsync(int id, BookingStatus status, CancellationToken cancellationToken = default)
    {
        var booking = await _unitOfWork.HotelBookings.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException(nameof(HotelBooking), id);

        booking.Status = status;
        _unitOfWork.HotelBookings.Update(booking);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}

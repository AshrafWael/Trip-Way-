using Travel.BLL.DTOs.Hotels;
using Travel.DAL.Entities;

namespace Travel.BLL.Interfaces;

public interface IHotelBookingService
{
    Task<IReadOnlyList<HotelBookingDto>> GetForUserAsync(string userId, CancellationToken cancellationToken = default);
    Task<HotelBookingDto> CreateAsync(string userId, CreateHotelBookingDto dto, CancellationToken cancellationToken = default);
    Task CancelAsync(int id, string userId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<HotelBookingDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task UpdateStatusAsync(int id, BookingStatus status, CancellationToken cancellationToken = default);
}

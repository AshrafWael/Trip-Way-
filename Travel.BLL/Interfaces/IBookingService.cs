using Travel.BLL.DTOs.Bookings;
using Travel.DAL.Entities;

namespace Travel.BLL.Interfaces;

public interface IBookingService
{
    Task<IReadOnlyList<BookingDto>> GetForUserAsync(string userId, CancellationToken cancellationToken = default);
    Task<BookingDto> GetByIdAsync(int id, string userId, bool isAdmin, CancellationToken cancellationToken = default);
    Task<BookingDto> CreateAsync(string userId, CreateBookingDto dto, CancellationToken cancellationToken = default);
    Task CancelAsync(int id, string userId, CancellationToken cancellationToken = default);

    // Admin
    Task<IReadOnlyList<BookingDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task UpdateStatusAsync(int id, BookingStatus status, CancellationToken cancellationToken = default);
}

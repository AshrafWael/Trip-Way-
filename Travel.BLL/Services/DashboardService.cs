using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Travel.BLL.DTOs.Admin;
using Travel.BLL.DTOs.Bookings;
using Travel.BLL.DTOs.Hotels;
using Travel.BLL.Interfaces;
using Travel.DAL.Entities;
using Travel.DAL.UnitOfWork;

namespace Travel.BLL.Services;

public class DashboardService : IDashboardService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IMapper _mapper;

    public DashboardService(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _userManager = userManager;
        _mapper = mapper;
    }

    public async Task<DashboardStatsDto> GetStatsAsync(CancellationToken cancellationToken = default)
    {
        var bookingsQuery = _unitOfWork.Bookings.Query();

        var totalBookings = await bookingsQuery.CountAsync(cancellationToken);
        var pendingBookings = await bookingsQuery.CountAsync(b => b.Status == BookingStatus.Pending, cancellationToken);
        var revenue = await bookingsQuery
            .Where(b => b.Status == BookingStatus.Confirmed || b.Status == BookingStatus.Completed)
            .SumAsync(b => (decimal?)b.TotalPrice, cancellationToken) ?? 0m;

        var recentBookings = await bookingsQuery
            .Include(b => b.TravelPackage)
            .OrderByDescending(b => b.CreatedAt)
            .Take(5)
            .ToListAsync(cancellationToken);

        var hotelBookingsQuery = _unitOfWork.HotelBookings.Query();

        var totalHotelBookings = await hotelBookingsQuery.CountAsync(cancellationToken);
        var pendingHotelBookings = await hotelBookingsQuery.CountAsync(b => b.Status == BookingStatus.Pending, cancellationToken);

        var recentHotelBookings = await hotelBookingsQuery
            .Include(b => b.Hotel)
            .OrderByDescending(b => b.CreatedAt)
            .Take(5)
            .ToListAsync(cancellationToken);

        // UserManager.Users is a plain IQueryable over the Identity store;
        // there's no async/cancellable overload on Queryable.Count/OrderBy/
        // Take, so this executes synchronously -- acceptable for an
        // admin-only, low-traffic dashboard endpoint.
        var totalUsers = _userManager.Users.Count();
        var recentUserEntities = _userManager.Users
            .OrderByDescending(u => u.CreatedAt)
            .Take(5)
            .ToList();

        var recentUsers = new List<UserSummaryDto>();
        foreach (var user in recentUserEntities)
        {
            var roles = await _userManager.GetRolesAsync(user);
            recentUsers.Add(new UserSummaryDto
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email ?? string.Empty,
                CreatedAt = user.CreatedAt,
                Roles = roles
            });
        }

        return new DashboardStatsDto
        {
            TotalUsers = totalUsers,
            TotalBookings = totalBookings,
            TotalPackages = await _unitOfWork.Packages.CountAsync(cancellationToken: cancellationToken),
            TotalDestinations = await _unitOfWork.Destinations.CountAsync(cancellationToken: cancellationToken),
            TotalHotels = await _unitOfWork.Hotels.CountAsync(cancellationToken: cancellationToken),
            TotalHotelBookings = totalHotelBookings,
            Revenue = revenue,
            PendingBookings = pendingBookings,
            PendingHotelBookings = pendingHotelBookings,
            RecentBookings = _mapper.Map<List<BookingDto>>(recentBookings),
            RecentHotelBookings = _mapper.Map<List<HotelBookingDto>>(recentHotelBookings),
            RecentUsers = recentUsers
        };
    }
}

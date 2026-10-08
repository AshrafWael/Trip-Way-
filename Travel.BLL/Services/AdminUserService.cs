using Microsoft.AspNetCore.Identity;
using Travel.BLL.DTOs.Admin;
using Travel.BLL.Exceptions;
using Travel.DAL.Entities;
using Travel.BLL.Interfaces;

namespace Travel.BLL.Services;

public class AdminUserService : IAdminUserService
{
    private readonly UserManager<ApplicationUser> _userManager;

    public AdminUserService(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<IReadOnlyList<UserSummaryDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        // Synchronous LINQ over UserManager.Users -- see DashboardService
        // for why that's an acceptable trade-off in an admin-only screen.
        var users = _userManager.Users.OrderByDescending(u => u.CreatedAt).ToList();

        var result = new List<UserSummaryDto>();
        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            result.Add(new UserSummaryDto
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email ?? string.Empty,
                PhoneNumber = user.PhoneNumber,
                Roles = roles,
                CreatedAt = user.CreatedAt,
                IsLockedOut = await _userManager.IsLockedOutAsync(user)
            });
        }

        return result;
    }

    public async Task SetLockedAsync(string userId, bool locked, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(userId)
            ?? throw new NotFoundException(nameof(ApplicationUser), userId);

        if (await _userManager.IsInRoleAsync(user, "Admin") && locked)
            throw new BusinessRuleException("An admin account cannot be locked from here.");

        await _userManager.SetLockoutEnabledAsync(user, true);
        await _userManager.SetLockoutEndDateAsync(user, locked ? DateTimeOffset.MaxValue : null);
    }
}

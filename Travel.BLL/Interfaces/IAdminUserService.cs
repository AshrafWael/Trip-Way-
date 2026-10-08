using Travel.BLL.DTOs.Admin;

namespace Travel.BLL.Interfaces;

public interface IAdminUserService
{
    Task<IReadOnlyList<UserSummaryDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task SetLockedAsync(string userId, bool locked, CancellationToken cancellationToken = default);
}

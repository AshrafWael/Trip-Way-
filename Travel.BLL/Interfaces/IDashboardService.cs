using Travel.BLL.DTOs.Admin;

namespace Travel.BLL.Interfaces;

public interface IDashboardService
{
    Task<DashboardStatsDto> GetStatsAsync(CancellationToken cancellationToken = default);
}

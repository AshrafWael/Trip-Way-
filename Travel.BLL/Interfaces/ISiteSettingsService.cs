using Travel.BLL.DTOs.Settings;

namespace Travel.BLL.Interfaces;

public interface ISiteSettingsService
{
    Task<SiteSettingsDto> GetAsync(CancellationToken cancellationToken = default);
    Task UpdateAsync(UpdateSiteSettingsDto dto, CancellationToken cancellationToken = default);
}

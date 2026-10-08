using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Travel.BLL.DTOs.Settings;
using Travel.BLL.Interfaces;
using Travel.DAL.Entities;
using Travel.DAL.UnitOfWork;

namespace Travel.BLL.Services;

public class SiteSettingsService : ISiteSettingsService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public SiteSettingsService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<SiteSettingsDto> GetAsync(CancellationToken cancellationToken = default)
    {
        var settings = await GetOrCreateRowAsync(cancellationToken);
        return _mapper.Map<SiteSettingsDto>(settings);
    }

    public async Task UpdateAsync(UpdateSiteSettingsDto dto, CancellationToken cancellationToken = default)
    {
        var settings = await GetOrCreateRowAsync(cancellationToken);
        _mapper.Map(dto, settings);
        _unitOfWork.SiteSettings.Update(settings);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    // The table always holds exactly one row. DbInitializer seeds it on
    // first run, but this fallback keeps things working even if that
    // seeding step was skipped for any reason.
    private async Task<SiteSettings> GetOrCreateRowAsync(CancellationToken cancellationToken)
    {
        var settings = await _unitOfWork.SiteSettings.Query(asNoTracking: false).FirstOrDefaultAsync(cancellationToken);
        if (settings is not null)
            return settings;

        settings = new SiteSettings
        {
            HeroTitle = "Discover Your Next Adventure",
            ArabicHeroTitle = "اكتشف مغامرتك القادمة",
            HeroSubtitle = "Explore the world's most beautiful destinations at the best prices with trusted service",
            ArabicHeroSubtitle = "استكشف أجمل الوجهات السياحية حول العالم بأفضل الأسعار وخدمة موثوقة",
            ContactPhone = "19XXX",
            ContactEmail = "info@example.com",
            ContactAddress = "Cairo, Egypt",
            ArabicContactAddress = "القاهرة، مصر",
            ContactHours = "Sat - Thu: 9 AM - 6 PM",
            ArabicContactHours = "السبت - الخميس: 9 صباحاً - 6 مساءً"
        };

        await _unitOfWork.SiteSettings.AddAsync(settings, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return settings;
    }
}

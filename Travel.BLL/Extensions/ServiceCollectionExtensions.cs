using System.Reflection;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Travel.BLL.Interfaces;
using Travel.BLL.Services;
using Travel.BLL.Settings;

namespace Travel.BLL.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddTravelBusinessLogic(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.SectionName));

        services.AddAutoMapper(cfg => { }, Assembly.GetExecutingAssembly());
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IDestinationService, DestinationService>();
        services.AddScoped<IPackageService, PackageService>();
        services.AddScoped<IBookingService, BookingService>();
        services.AddScoped<IHotelService, HotelService>();
        services.AddScoped<IHotelBookingService, HotelBookingService>();
        services.AddScoped<IWishlistService, WishlistService>();
        services.AddScoped<IReviewService, ReviewService>();
        services.AddScoped<IBlogService, BlogService>();
        services.AddScoped<IContactService, ContactService>();
        services.AddScoped<IDashboardService, DashboardService>();
        services.AddScoped<IAdminUserService, AdminUserService>();
        services.AddScoped<INewsletterService, NewsletterService>();
        services.AddScoped<ISiteSettingsService, SiteSettingsService>();

        return services;
    }
}

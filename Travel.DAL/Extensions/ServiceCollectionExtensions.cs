using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Travel.DAL.Data.Context;
using Travel.DAL.Data.Seed;
using Travel.DAL.Repositories;
using Travel.DAL.UnitOfWork;

namespace Travel.DAL.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddTravelDataAccess(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));


        services.Configure<SeedAdminOptions>(configuration.GetSection(SeedAdminOptions.SectionName));

        services.AddScoped<IUnitOfWork, UnitOfWork.UnitOfWork>();
        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
        services.AddScoped<IPackageRepository, PackageRepository>();

        return services;
    }
}

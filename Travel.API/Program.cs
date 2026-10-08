using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using System.Text;
using System.Threading.RateLimiting;
using Travel.API.Filters;
using Travel.BLL.Extensions;
using Travel.BLL.Settings;
using Travel.DAL.Data.Context;
using Travel.DAL.Data.Seed;
using Travel.DAL.Entities;
using Travel.DAL.Extensions;

var builder = WebApplication.CreateBuilder(args);

// ---------------------------------------------------------------------
// Phase 4: all domain API controllers are now live (Destinations,
// Packages, Bookings, Wishlist, Reviews, Blog, Contact + their Admin
// counterparts), protected with [Authorize(Roles = "Admin")] where the
// spec calls for it, and every exception is funneled through
// ExceptionHandlingMiddleware into the platform-wide ApiResponse shape.
//
// Hardening pass: rate limiting (global + a stricter policy on
// auth endpoints, since those are the most attractive brute-force
// target) and structured HTTP request logging, both using ASP.NET
// Core's built-in middleware -- no extra NuGet packages needed.
// ---------------------------------------------------------------------

builder.Services.AddTravelDataAccess(builder.Configuration);
builder.Services.AddTravelBusinessLogic(builder.Configuration);

builder.Services.AddControllers(options =>
{
    options.Filters.Add<ValidationFilter>();
});
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddHttpLogging(options =>
{
    options.LoggingFields = HttpLoggingFields.RequestMethod | HttpLoggingFields.RequestPath
        | HttpLoggingFields.ResponseStatusCode | HttpLoggingFields.Duration;
    // Deliberately excludes headers/bodies -- request/response content can
    // carry passwords, tokens, or personal data, which don't belong in logs.
});

builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

    // Global default: keyed per client IP so one abusive caller can't
    // starve everyone else, generous enough not to bother normal browsing.
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 100,
                Window = TimeSpan.FromMinutes(1),
                QueueLimit = 0
            }));

    // Tighter policy for register/login -- these are the endpoints a
    // credential-stuffing or brute-force script would actually hit.
    options.AddPolicy("auth", context =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 10,
                Window = TimeSpan.FromMinutes(1),
                QueueLimit = 0
            }));
});

var jwtSettings = builder.Configuration.GetSection(JwtSettings.SectionName).Get<JwtSettings>()
    ?? new JwtSettings();
builder.Services
    .AddIdentity<ApplicationUser, IdentityRole>(options =>
    {
        options.Password.RequiredLength = 8;
        options.User.RequireUniqueEmail = true;
    })
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();
builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        // Signing key comes from configuration (user-secrets / env vars)
        // only -- see JwtSettings and ServiceCollectionExtensions.
        var secret = builder.Configuration["JwtSettings:Secret"] ?? string.Empty;

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidAudience = jwtSettings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret)),
            ClockSkew = TimeSpan.FromMinutes(1)
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Travel API",
        Version = "v1"
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter: Bearer {your JWT token}"
    });

    options.AddSecurityRequirement(document =>
        new OpenApiSecurityRequirement
        {
            [new OpenApiSecuritySchemeReference("Bearer", document)] = []
        });
});
#region
//builder.Services.AddSwaggerGen(options =>
//{
//    options.SwaggerDoc("v1", new OpenApiInfo { Title = "Travel API", Version = "v1" });

//    var securityScheme = new OpenApiSecurityScheme
//    {
//        Name = "Authorization",
//        Type = SecuritySchemeType.Http,
//        Scheme = "Bearer",
//        BearerFormat = "JWT",
//        In = ParameterLocation.Header,
//        Description = "Enter: Bearer {your JWT token}",
//        Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
//    };

//    options.AddSecurityDefinition("Bearer", securityScheme);
//    options.AddSecurityRequirement(new OpenApiSecurityRequirement { { securityScheme, Array.Empty<string>() } });
//});
#endregion

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    // Idempotent dev-time seeding only -- production data is created via
    // migrations + a controlled admin workflow, not on every boot.
    using var scope = app.Services.CreateScope();
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<ApplicationDbContext>();
    var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
    var seedOptions = services.GetRequiredService<Microsoft.Extensions.Options.IOptions<SeedAdminOptions>>();
    var logger = services.GetRequiredService<ILogger<Program>>();

    await context.Database.MigrateAsync();
    await DbInitializer.SeedAsync(context, userManager, roleManager, seedOptions, logger);
}

app.UseHttpsRedirection();
app.UseHttpLogging();
app.UseMiddleware<Travel.API.Middleware.ExceptionHandlingMiddleware>();
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();

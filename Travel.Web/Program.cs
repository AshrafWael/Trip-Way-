using System.Globalization;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Localization;
using Travel.Web.Resources;
using Travel.Web.Services.ApiClient;

var builder = WebApplication.CreateBuilder(args);

// ---------------------------------------------------------------------
// Phase 5: the MVC front end talks to Travel.API only -- never to
// Travel.DAL or SQL Server directly. A cookie holds the signed-in
// person's identity plus the JWT issued by the API at login time;
// JwtAuthHandler re-attaches that JWT to every outgoing API call.
//
// Localization: infrastructure for ar-EG (default) / en-US is wired up
// here and used throughout the site (layout, every view, and every
// form field label) via IStringLocalizer<SharedResource>. Form field
// labels come from [Display(Name = "Field_X")] attributes on the DTOs
// and ViewModels, resolved against the same SharedResource resx pair
// below -- see DataAnnotationLocalizerProvider.
// ---------------------------------------------------------------------

builder.Services.AddControllersWithViews()
    .AddViewLocalization()
    .AddDataAnnotationsLocalization(options =>
    {
        options.DataAnnotationLocalizerProvider = (_, factory) => factory.Create(typeof(SharedResource));
    });
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

var supportedCultures = new[] { new CultureInfo("ar-EG"), new CultureInfo("en-US") };
var localizationOptions = new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture("ar-EG"),
    SupportedCultures = supportedCultures,
    SupportedUICultures = supportedCultures
};
// By default, RequestLocalizationOptions also registers an
// Accept-Language-header provider, which almost always finds a match
// (most browsers send "en-US" or similar) and so silently overrides
// DefaultRequestCulture on every first visit -- this is why the site
// showed up in English for everyone regardless of the ar-EG default.
// Only the explicit language-switcher cookie (and, if present, a
// culture query string) should ever change the language; the browser's
// own preference should not.
localizationOptions.RequestCultureProviders = new List<IRequestCultureProvider>
{
    new QueryStringRequestCultureProvider(),
    new CookieRequestCultureProvider()
};

builder.Services.AddHttpContextAccessor();

// Header name for the wishlist AJAX call's CSRF token (see
// wwwroot/js/site.js) -- the default antiforgery setup only looks at a
// form field, which a fetch() POST doesn't have.
builder.Services.AddAntiforgery(options => options.HeaderName = "X-CSRF-TOKEN");

builder.Services.Configure<ApiSettings>(builder.Configuration.GetSection(ApiSettings.SectionName));

builder.Services.AddTransient<JwtAuthHandler>();
builder.Services.AddHttpClient<ITravelApiClient, TravelApiClient>((serviceProvider, client) =>
{
    var apiSettings = builder.Configuration.GetSection(ApiSettings.SectionName).Get<ApiSettings>()
        ?? new ApiSettings();
    client.BaseAddress = new Uri(apiSettings.BaseUrl);
})
.AddHttpMessageHandler<JwtAuthHandler>();

builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromHours(1);
        options.SlidingExpiration = true;
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
    });

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRequestLocalization(localizationOptions);
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

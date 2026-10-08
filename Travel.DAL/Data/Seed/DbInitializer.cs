using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Travel.DAL.Data.Context;
using Travel.DAL.Entities;

namespace Travel.DAL.Data.Seed;

/// <summary>
/// Idempotent startup seeding: safe to run every time the app starts,
/// since every step checks for existing data before inserting.
/// </summary>
public static class DbInitializer
{
    private static readonly string[] Roles = { "Admin", "Customer" };

    public static async Task SeedAsync(
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager,
        IOptions<SeedAdminOptions> seedAdminOptions,
        ILogger logger)
    {
        await SeedRolesAsync(roleManager);
        await SeedAdminUserAsync(userManager, seedAdminOptions.Value, logger);
        await SeedDestinationsAndPackagesAsync(context);
        await SeedHotelsAsync(context);
        await SeedBlogPostsAsync(context, seedAdminOptions.Value.Email, userManager);
        await SeedSiteSettingsAsync(context);
    }

    private static async Task SeedHotelsAsync(ApplicationDbContext context)
    {
        if (context.Hotels.Any())
            return;

        var destinations = context.Destinations.ToList();
        if (destinations.Count == 0)
            return;

        var cairo = destinations.FirstOrDefault(d => d.Name == "Cairo") ?? destinations[0];
        var sharm = destinations.FirstOrDefault(d => d.Name == "Sharm El Sheikh") ?? destinations[0];
        var istanbul = destinations.FirstOrDefault(d => d.Name == "Istanbul") ?? destinations[0];

        var hotels = new List<Hotel>
        {
            new()
            {
                Name = "Nile Grand Hotel", ArabicName = "فندق النيل الكبير",
                Description = "An elegant riverside hotel with panoramic views of the Nile and easy access to Cairo's landmarks.",
                ArabicDescription = "فندق أنيق على ضفاف النيل يتمتع بإطلالات بانورامية ويسهل الوصول منه إلى معالم القاهرة.",
                DestinationId = cairo.Id,
                Address = "Corniche El Nil, Cairo", ArabicAddress = "كورنيش النيل، القاهرة",
                StarRating = 5, PricePerNight = 120m, DiscountPricePerNight = 99m,
                TotalRooms = 80, AvailableRooms = 80, IsFeatured = true,
                MainImageUrl = "https://images.unsplash.com/photo-1566073771259-6a8506099945?w=800&q=80"
            },
            new()
            {
                Name = "Red Sea Resort & Spa", ArabicName = "منتجع البحر الأحمر والسبا",
                Description = "A beachfront resort in Sharm El Sheikh with a private beach, diving center, and spa.",
                ArabicDescription = "منتجع على الشاطئ في شرم الشيخ يضم شاطئًا خاصًا ومركز غوص وسبا.",
                DestinationId = sharm.Id,
                Address = "Naama Bay, Sharm El Sheikh", ArabicAddress = "خليج نعمة، شرم الشيخ",
                StarRating = 4, PricePerNight = 150m,
                TotalRooms = 120, AvailableRooms = 120, IsFeatured = true,
                MainImageUrl = "https://images.unsplash.com/photo-1571003123894-1f0594d2b5d9?w=800&q=80"
            },
            new()
            {
                Name = "Bosphorus View Hotel", ArabicName = "فندق إطلالة البوسفور",
                Description = "A boutique hotel in the heart of Istanbul with views of the Bosphorus strait.",
                ArabicDescription = "فندق بوتيك في قلب إسطنبول يطل على مضيق البوسفور.",
                DestinationId = istanbul.Id,
                Address = "Sultanahmet, Istanbul", ArabicAddress = "السلطان أحمد، إسطنبول",
                StarRating = 4, PricePerNight = 135m, DiscountPricePerNight = 115m,
                TotalRooms = 60, AvailableRooms = 60, IsFeatured = false,
                MainImageUrl = "https://images.unsplash.com/photo-1520250497591-112f2f40a3f4?w=800&q=80"
            }
        };

        await context.Hotels.AddRangeAsync(hotels);
        await context.SaveChangesAsync();
    }

    private static async Task SeedSiteSettingsAsync(ApplicationDbContext context)
    {
        if (context.SiteSettings.Any())
            return;

        context.SiteSettings.Add(new SiteSettings
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
        });

        await context.SaveChangesAsync();
    }

    private static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
    {
        foreach (var role in Roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new IdentityRole(role));
        }
    }

    private static async Task SeedAdminUserAsync(
        UserManager<ApplicationUser> userManager,
        SeedAdminOptions options,
        ILogger logger)
    {
        if (string.IsNullOrWhiteSpace(options.Email) || string.IsNullOrWhiteSpace(options.Password))
        {
            logger.LogWarning(
                "SeedAdmin:Email / SeedAdmin:Password are not configured — skipping admin user seeding. " +
                "Set them via dotnet user-secrets before relying on an admin login.");
            return;
        }

        if (await userManager.FindByEmailAsync(options.Email) is not null)
            return;

        var admin = new ApplicationUser
        {
            UserName = options.Email,
            Email = options.Email,
            EmailConfirmed = true,
            FullName = options.FullName,
            CreatedAt = DateTime.UtcNow
        };

        var result = await userManager.CreateAsync(admin, options.Password);
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(admin, "Admin");
        }
        else
        {
            logger.LogError(
                "Failed to seed admin user: {Errors}",
                string.Join("; ", result.Errors.Select(e => e.Description)));
        }
    }

    private static async Task SeedDestinationsAndPackagesAsync(ApplicationDbContext context)
    {
        if (context.Destinations.Any())
            return;

        var destinations = new List<Destination>
        {
            new()
            {
                Name = "Cairo", ArabicName = "القاهرة",
                Description = "Egypt's sprawling capital, home to the Pyramids of Giza and the Egyptian Museum.",
                ArabicDescription = "عاصمة مصر الكبرى، موطن أهرامات الجيزة والمتحف المصري.",
                Country = "Egypt", City = "Cairo", IsFeatured = true,
                ImageUrl = "https://images.unsplash.com/photo-1568322445389-f64ac9c68f5f?w=800&q=80"
            },
            new()
            {
                Name = "Sharm El Sheikh", ArabicName = "شرم الشيخ",
                Description = "A Red Sea resort town known for its coral reefs and diving spots.",
                ArabicDescription = "مدينة منتجعية على البحر الأحمر تشتهر بالشعاب المرجانية ومواقع الغوص.",
                Country = "Egypt", City = "Sharm El Sheikh", IsFeatured = true,
                ImageUrl = "https://images.unsplash.com/photo-1544551763-46a013bb70d5?w=800&q=80"
            },
            new()
            {
                Name = "Istanbul", ArabicName = "إسطنبول",
                Description = "A transcontinental city straddling Europe and Asia, rich in Ottoman history.",
                ArabicDescription = "مدينة عابرة للقارات تجمع بين أوروبا وآسيا وتزخر بالتاريخ العثماني.",
                Country = "Turkey", City = "Istanbul", IsFeatured = true,
                ImageUrl = "https://images.unsplash.com/photo-1524231757912-21f4fe3a7200?w=800&q=80"
            }
        };

        await context.Destinations.AddRangeAsync(destinations);
        await context.SaveChangesAsync();

        var packages = new List<TravelPackage>
        {
            new()
            {
                Title = "Cairo Highlights: Pyramids & Museums",
                ArabicTitle = "أبرز معالم القاهرة: الأهرامات والمتاحف",
                Description = "A 4-day guided tour through Cairo's ancient wonders and modern culture.",
                ArabicDescription = "جولة مصحوبة بمرشد لمدة 4 أيام عبر عجائب القاهرة القديمة وثقافتها الحديثة.",
                DestinationId = destinations[0].Id,
                DurationDays = 4, DurationNights = 3,
                Price = 650m, DiscountPrice = 549m,
                MaxTravelers = 15, AvailableSeats = 15,
                IsFeatured = true,
                MainImageUrl = "https://images.unsplash.com/photo-1503177119275-0aa32b3a9368?w=800&q=80"
            },
            new()
            {
                Title = "Red Sea Diving Escape",
                ArabicTitle = "رحلة غوص في البحر الأحمر",
                Description = "5 days of diving, snorkeling and beach relaxation in Sharm El Sheikh.",
                ArabicDescription = "5 أيام من الغوص والسباحة والاسترخاء على الشاطئ في شرم الشيخ.",
                DestinationId = destinations[1].Id,
                DurationDays = 5, DurationNights = 4,
                Price = 720m,
                MaxTravelers = 12, AvailableSeats = 12,
                IsFeatured = true,
                MainImageUrl = "https://images.unsplash.com/photo-1518509562904-e7ef99cdcc86?w=800&q=80"
            },
            new()
            {
                Title = "Istanbul: Two Continents in One Trip",
                ArabicTitle = "إسطنبول: قارتان في رحلة واحدة",
                Description = "6 days exploring the Bosphorus, the Grand Bazaar and Ottoman palaces.",
                ArabicDescription = "6 أيام لاستكشاف مضيق البوسفور والبازار الكبير والقصور العثمانية.",
                DestinationId = destinations[2].Id,
                DurationDays = 6, DurationNights = 5,
                Price = 980m, DiscountPrice = 899m,
                MaxTravelers = 20, AvailableSeats = 20,
                IsFeatured = false,
                MainImageUrl = "https://images.unsplash.com/photo-1541432901042-2d8bd64b4a9b?w=800&q=80"
            }
        };

        await context.TravelPackages.AddRangeAsync(packages);
        await context.SaveChangesAsync();
    }

    private static async Task SeedBlogPostsAsync(
        ApplicationDbContext context,
        string adminEmail,
        UserManager<ApplicationUser> userManager)
    {
        if (context.BlogPosts.Any() || string.IsNullOrWhiteSpace(adminEmail))
            return;

        var admin = await userManager.FindByEmailAsync(adminEmail);
        if (admin is null)
            return;

        context.BlogPosts.Add(new BlogPost
        {
            Title = "5 Reasons to Visit Egypt This Year",
            ArabicTitle = "5 أسباب لزيارة مصر هذا العام",
            Content = "From ancient wonders to Red Sea coastlines, Egypt offers something for every traveler...",
            ArabicContent = "من العجائب القديمة إلى سواحل البحر الأحمر، تقدم مصر شيئًا لكل مسافر...",
            ImageUrl = "https://images.unsplash.com/photo-1539768942893-daf53e448371?w=800&q=80",
            AuthorId = admin.Id,
            IsPublished = true,
            PublishedAt = DateTime.UtcNow
        });

        await context.SaveChangesAsync();
    }
}

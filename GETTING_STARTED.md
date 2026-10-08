# Getting Started

This is a from-scratch solution written across several phases — see
`README.md` for the architecture. Nothing here has been build-verified
(the sandbox that produced it has no .NET SDK), so treat step 1 as the
first real test of the code.

## 1. Prerequisites

- .NET 10 SDK
- SQL Server (LocalDB is fine for development) or SQL Server Express
- `dotnet-ef` tool: `dotnet tool install --global dotnet-ef` (skip if already installed)

## 2. Restore and configure secrets

From the solution root:

```bash
dotnet restore Travel.sln
```

Then, from `Travel.API`, set the secrets that are deliberately **not**
committed anywhere in the repo:

```bash
cd Travel.API
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=(localdb)\mssqllocaldb;Database=TravelDb;Trusted_Connection=True;TrustServerCertificate=True"
dotnet user-secrets set "JwtSettings:Secret" "<a-long-random-string-32-chars-or-more>"
dotnet user-secrets set "SeedAdmin:Email" "admin@example.com"
dotnet user-secrets set "SeedAdmin:Password" "<a-strong-password>"
```

(Any SQL Server connection string works — adjust if you're not using LocalDB.)

## 3. Create the database

Still from `Travel.API`:

```bash
dotnet ef migrations add InitialCreate --project ../Travel.DAL --startup-project .
dotnet ef database update --project ../Travel.DAL --startup-project .
```

This creates the schema. Seed data (roles, the admin account, a few
sample destinations/packages, one blog post) is inserted automatically
the first time you run the API in Development mode — no separate command.

## 4. Run both projects

You need **both** running at once, since `Travel.Web` calls `Travel.API`
over HTTP rather than touching the database itself.

```bash
# terminal 1
cd Travel.API
dotnet run

# terminal 2
cd Travel.Web
dotnet run
```

Note the port `Travel.API` starts on (shown in the console, e.g.
`https://localhost:7001`) and make sure it matches
`Travel.Web/appsettings.json` → `ApiSettings:BaseUrl`. Update that file if
the ports don't line up, then restart `Travel.Web`.

Open the URL `Travel.Web` prints (typically `https://localhost:7157` or
similar) in your browser.

## 5. Using the site as a customer

1. **Browse without an account** — the homepage, Destinations, and
   Packages pages (with filters/sorting/pagination) all work anonymously.
2. **Create an account** — click "إنشاء حساب" (top right) and register.
   You're signed in immediately after.
3. **Book a trip** — open any package's details page, pick a travel date
   and number of travelers (the total price updates live), and click
   "احجز الآن" (Book Now). The booking is created with **Pending** status.
4. **Add to wishlist** — the heart button on the same page adds the
   package to your wishlist without leaving the page.
5. **View your bookings** — "حجوزاتي" in the navbar. You can cancel a
   Pending or Confirmed booking from there.
6. **View your wishlist** — "المفضلة" in the navbar.
7. **Leave a review** — reviews are only allowed on **Completed**
   bookings (an admin has to mark a booking Completed first — see below),
   at which point a "أضف تقييماً" button appears next to that booking on
   the My Bookings page. Submitted reviews are hidden from the public
   package page until an admin approves them.
8. **Read the blog / contact the company** — "المدونة" and "اتصل بنا" in
   the navbar are both fully live pages now.
9. **Subscribe to the newsletter** — the form at the bottom of the
   homepage actually submits (to a real `NewsletterSubscriber` table) and
   shows inline confirmation.
10. **Switch language** — the "EN"/"AR" button in the navbar switches the
    site language via a cookie. This is only fully translated on the
    homepage and shared layout (nav/footer) right now — see section 8.

## 6. Using the Admin area

1. Log in with the `SeedAdmin:Email` / `SeedAdmin:Password` you set in
   step 2.
2. Go to `/Admin` (or click through — the admin-only nav items only
   appear for an Admin-role account; as a shortcut you can also just
   navigate to `https://localhost:<web-port>/Admin`).
3. From the sidebar:
   - **الرئيسية (Dashboard)** — totals, revenue, pending bookings, recent
     activity.
   - **الوجهات / الرحلات (Destinations / Packages)** — create new ones,
     edit existing ones, or "delete" (which deactivates rather than
     hard-deletes, since bookings/reviews reference them).
   - **الحجوزات (Bookings)** — every booking in the system; change a
     booking's status here (e.g. Pending → Confirmed → Completed) — this
     is the step that unlocks that customer's ability to leave a review.
   - **المستخدمون (Users)** — see everyone registered, lock/unlock an
     account (Admin accounts can't be locked from here).
   - **التقييمات (Reviews)** — approve or delete submitted reviews.
   - **المدونة (Blog)** — write/publish/unpublish articles.
   - **رسائل التواصل (Contact Messages)** — anything submitted through the
     public contact form.

## 7. Running the automated tests

```bash
dotnet test Travel.Tests/Travel.Tests.csproj
```

Covers the trickiest business rules (booking seat-count/pricing/
cancellation, review eligibility, key FluentValidation rules) — not a full
suite, and there's no integration-test layer yet.

## 8. What's genuinely not finished

Being upfront about this so you don't go looking for it:

- **Localization** is only applied to the shared layout and the Home page.
  Every other view (Destinations, Packages, Bookings, Admin, etc.) still
  has hardcoded Arabic strings. The infrastructure (`IStringLocalizer`,
  resource files, language switcher) is fully in place — extending the
  same pattern to the rest of the views is mechanical but wasn't done for
  all ~28 of them.
- **No integration/end-to-end tests** — only unit tests exist.
- **No container registry push** in CI — the GitHub Actions workflow
  builds Docker images but doesn't push them anywhere; that needs registry
  credentials specific to wherever this actually gets deployed.
- **Admin user management** is lock/unlock only — editing a user's profile
  fields isn't built (matches what the original brief asked for, "manage
  users", without further detail).

None of this is silently broken — each item is a feature/extension that
hasn't been built, not a bug in something that has.

## 9. Running everything with Docker Compose (alternative to steps 2-4)

```bash
cp .env.example .env   # fill in DB_SA_PASSWORD, JWT_SECRET, SEED_ADMIN_EMAIL, SEED_ADMIN_PASSWORD
docker compose up --build
```

`Travel.Web` → `http://localhost:8080`, `Travel.API` → `http://localhost:8081`.
This still requires the migration step (3) to be run once against the
containerized SQL Server before the site has any data.

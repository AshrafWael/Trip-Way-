# Travel & Tourism Platform — Solution Scaffold

## Architecture

```
Travel.sln
├── Travel.API   ASP.NET Core Web API — controllers, JWT, Swagger, exception middleware
├── Travel.BLL   Business logic — DTOs, service interfaces/implementations, AutoMapper, validators
├── Travel.DAL   Data access — DbContext, entities, EF Core configurations, repositories, Unit of Work
└── Travel.Web   ASP.NET Core MVC — public RTL site + Admin area, talks to Travel.API only via a typed HttpClient
```

Dependency direction: `Travel.API → Travel.BLL → Travel.DAL`. `Travel.Web` references
`Travel.BLL` only for its DTO contracts — it never touches `Travel.DAL` or SQL Server
directly; all data flows through the API over HTTP.

## Database design (Phase 1 entities)

- **ApplicationUser** (extends `IdentityUser`) — 1‑to‑many with Bookings, Wishlist, Reviews, BlogPosts
- **Destination** — 1‑to‑many with TravelPackage
- **TravelPackage** — many‑to‑1 Destination; 1‑to‑many PackageImage, Booking, Wishlist, Review
- **PackageImage** — many‑to‑1 TravelPackage (gallery images)
- **Booking** — many‑to‑1 ApplicationUser and TravelPackage; `BookingStatus` enum (Pending/Confirmed/Cancelled/Completed)
- **Wishlist** — join entity between ApplicationUser and TravelPackage
- **Review** — many‑to‑1 ApplicationUser and TravelPackage; moderated via `IsApproved`
- **BlogPost** — many‑to‑1 ApplicationUser (Author)
- **ContactMessage** — standalone inbox entity for the contact form

Fluent API configurations (keys, indexes, decimal precision, delete behavior) land in
Phase 2 as `Travel.DAL/Data/Configurations/*Configuration.cs`, applied via
`ApplyConfigurationsFromAssembly`.

## What's included in this Phase 1 drop

- Solution + 4 projects wired together with the correct project references
- NuGet package references for EF Core, Identity, JWT, AutoMapper, FluentValidation, Swashbuckle
- All 9 entity model classes with navigation properties
- Bare-minimum `Program.cs` in API and Web so both projects run (no DB, auth, or business logic yet)
- `appsettings.json` with **empty placeholders only** — no connection strings, JWT secrets,
  or admin credentials are committed anywhere in this scaffold

## Required local setup (secrets)

Before Phase 2, initialize user secrets so nothing sensitive ever reaches source control:

```bash
cd Travel.API
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=.;Database=TravelDb;Trusted_Connection=True;TrustServerCertificate=True"
dotnet user-secrets set "JwtSettings:Secret" "<generate-a-long-random-string>"
dotnet user-secrets set "SeedAdmin:Email" "admin@example.com"
dotnet user-secrets set "SeedAdmin:Password" "<a-strong-password>"
```

## Phase 2 — data access layer (this drop)

- Fluent API configuration class per entity in `Travel.DAL/Data/Configurations`
  (keys, max lengths, decimal precision, indexes, unique constraints, check
  constraints for things like "discount price ≤ price" and "rating 1–5", and
  a `DeleteBehavior` chosen per relationship so a delete can't silently wipe
  booking/review history or hit a multiple-cascade-path error)
- `ApplicationDbContext : IdentityDbContext<ApplicationUser>` — configurations
  are applied automatically via `ApplyConfigurationsFromAssembly`, so adding a
  new entity's config later is a one-file change, nothing to register by hand
- `IGenericRepository<T>` / `GenericRepository<T>` — the CRUD + query surface
  shared by every entity, including an `IQueryable<T> Query()` escape hatch for
  the service layer to compose paging/sorting without a bespoke method per
  combination
- `IPackageRepository` / `PackageRepository` — the one repository specialized
  beyond the generic one, because package listing always needs
  Destination + Images eagerly loaded plus multi-field search/sort/paging;
  `SearchAsync(...)` backs the future `GET /api/packages` filters and
  `GetWithDetailsAsync(id)` backs the package details page
- `IUnitOfWork` / `UnitOfWork` — wraps all repositories behind one
  `SaveChangesAsync`, so multi-entity operations (e.g. a booking that also
  decrements `AvailableSeats`) commit atomically
- `DbInitializer` — idempotent seeding for roles (Admin/Customer), the admin
  user (from `SeedAdmin:Email` / `SeedAdmin:Password` in user-secrets, never
  hardcoded), a few destinations/packages, and one blog post
- `Travel.DAL/Extensions/ServiceCollectionExtensions.AddTravelDataAccess(...)`
  registers the DbContext, Identity, and all repositories/UoW in one call;
  `Travel.API`'s `Program.cs` now calls it and, in Development, runs
  `Database.MigrateAsync()` + `DbInitializer.SeedAsync()`

### You still need to generate the initial migration yourself

This sandbox has no .NET SDK installed, so the migration hasn't been generated
or build-verified here. From `Travel.API`, after the `dotnet user-secrets`
setup above:

```bash
dotnet ef migrations add InitialCreate --project ../Travel.DAL --startup-project .
dotnet ef database update --project ../Travel.DAL --startup-project .
```

(Install the tool first if needed: `dotnet tool install --global dotnet-ef`.)

## Phase 3 — DTOs, services, validation, JWT auth (this drop)

- **DTOs** under `Travel.BLL/DTOs`, grouped by feature (Auth, Destinations,
  Packages, Bookings, Wishlist, Reviews, Blog, Contact) plus a shared
  `ApiResponse<T>` envelope and `PagedResultDto<T>` — no EF entity is ever
  returned from the API directly
- **AutoMapper** — a single `MappingProfile` covers every entity↔DTO pair;
  package listing/search maps to the lightweight `PackageDto`, package
  details maps to `PackageDetailsDto` (adds images + approved reviews)
- **Service layer** — `IAuthService`, `IDestinationService`, `IPackageService`,
  `IBookingService`, `IWishlistService`, `IReviewService`, `IBlogService`,
  `IContactService`, all built on `IUnitOfWork` so controllers stay thin.
  Notable business rules enforced here (not just in the DB constraints):
  - Booking a package can't request more travelers than `AvailableSeats`;
    seats are decremented and the booking row inserted in the same
    `SaveChangesAsync` so they can't drift apart
  - Cancelling a booking releases the seats back to the package
  - A review can only be left after a **Completed** booking for that
    package, and only once per user/package pair
  - Destinations/packages are **soft-deleted** (`IsActive = false`) rather
    than removed, since bookings/reviews reference them for history
- **FluentValidation** — validators for the "create" DTOs that need cross-field
  or business rules DataAnnotations can't express cleanly (discount ≤ price,
  available seats ≤ max travelers, travel date not in the past, password
  complexity). A `ValidationFilter` action filter in `Travel.API` resolves the
  matching `IValidator<T>` for any action argument automatically and returns
  a 400 in the `ApiResponse` envelope on failure — no per-controller
  boilerplate. `[ApiController]`'s built-in DataAnnotations check still
  covers the simpler per-field rules.
- **JWT authentication** — `ITokenService`/`TokenService` issues tokens with
  the user's id, email, name and role claims; `AuthService` handles
  register/login via `UserManager` (`CheckPasswordAsync`, no `SignInManager`
  needed since `Travel.BLL` is a plain class library); `POST /api/auth/register`
  and `POST /api/auth/login` are live in `AuthController`. `Program.cs` now
  configures `AddJwtBearer` reading `JwtSettings:Issuer/Audience` from
  config and `JwtSettings:Secret` from user-secrets, and Swagger has a
  Bearer auth definition so protected endpoints (added in Phase 4) can be
  tested with the "Authorize" button.
- Two exception types (`NotFoundException`, `BusinessRuleException`, and
  `AuthenticationFailedException`) are thrown from the service layer; they
  aren't mapped to HTTP status codes yet — that's the centralized
  exception-handling middleware in Phase 4.

### Secrets needed for Phase 3 to run

In addition to the Phase 2 `dotnet user-secrets` commands, nothing new is
required — `JwtSettings:Secret` was already listed there. Make sure it's a
long, random string (32+ characters) since it signs every token.

## Phase 4 — API controllers, Swagger, exception middleware (this drop)

- **`ExceptionHandlingMiddleware`** — the single place that turns
  `NotFoundException` → 404, `AuthenticationFailedException` → 401,
  `BusinessRuleException` → 400, `UnauthorizedAccessException` → 403, and
  anything else → 500 (logged with full detail, but the client only ever
  sees a generic message for that last case). Every response uses the same
  `ApiResponse<T>` envelope from Phase 3.
- **Every domain controller from the spec is now live**, all returning
  `ApiResponse<T>` and never an EF entity:

  | Area | Public | Customer (`[Authorize]`) | Admin (`[Authorize(Roles = "Admin")]`) |
  |---|---|---|---|
  | Destinations | `GET /api/destinations`, `GET /api/destinations/{id}` | — | `POST` / `PUT /{id}` / `DELETE /{id}` |
  | Packages | `GET /api/packages` (search/filter/sort/page), `GET /api/packages/{id}` | — | `POST` / `PUT /{id}` / `DELETE /{id}` |
  | Bookings | — | `GET /api/bookings/my`, `GET /{id}`, `POST`, `PUT /{id}/cancel` | `GET /api/admin/bookings`, `PUT /{id}/status` |
  | Wishlist | — | `GET /api/wishlist`, `POST /{packageId}`, `DELETE /{packageId}` | — |
  | Reviews | `GET /api/packages/{packageId}/reviews` | `POST /api/packages/{packageId}/reviews` | `PUT /api/admin/reviews/{id}/approve`, `DELETE /api/admin/reviews/{id}` |
  | Blog | `GET /api/blog`, `GET /api/blog/{id}` | — | `GET /api/blog/admin/all`, `POST`, `PUT /{id}`, `DELETE /{id}` |
  | Contact | `POST /api/contact` | — | `GET /api/contact`, `PUT /{id}/read` |
  | Auth | `POST /api/auth/register`, `POST /api/auth/login` | — | — |

- Booking and review creation read the user id from the JWT (`ClaimsPrincipal.GetUserId()`,
  a small extension in `Travel.API/Extensions`) rather than trusting a value
  from the request body.
- The review-creation route takes `packageId` from the URL and overwrites
  whatever the client sent in the body, so the two can't disagree.
- Swagger's Bearer button (added in Phase 3) now has real protected
  endpoints to test against — try `POST /api/auth/login` with the seeded
  admin credentials, then "Authorize" with the returned token.

## Phase 5 — Travel.Web: API client, cookie auth, first public pages (this drop)

- **`ITravelApiClient` / `TravelApiClient`** (`Travel.Web/Services/ApiClient`) —
  the only way `Travel.Web` talks to `Travel.API`. Every response is
  unwrapped from the `ApiResponse<T>` envelope; a non-success response
  throws `ApiException` (status code + message + field errors) so
  controllers can show the API's own validation messages instead of a
  generic error page.
- **`JwtAuthHandler`** — a `DelegatingHandler` registered on the typed
  `HttpClient` that reads the JWT out of the signed-in cookie's claims and
  attaches it as `Authorization: Bearer ...` to every outgoing API call, so
  a person only logs in once even though two separate ASP.NET Core apps are
  involved.
- **Cookie authentication** — `AccountController` (`Login` / `Register` /
  `Logout` / `AccessDenied`) calls the API, then signs the person into a
  cookie carrying their id/name/email/roles **and** the API's JWT as a
  claim. The cookie's `ExpiresUtc` is pinned to the JWT's own expiry since
  there's no refresh-token flow yet — noted in the controller as a known
  simplification to revisit if session length becomes a UX problem.
- **First public pages**, all in Arabic/RTL (Tajawal font, Bootstrap 5 RTL
  build), sharing one premium `_Layout.cshtml` with a sticky navbar and a
  full footer:
  - **Home** — hero with a destination/price search box, featured
    destinations, featured packages, a "why choose us" strip, and a
    newsletter section
  - **Destinations** — grid listing + a details page linking into the
    filtered package list for that destination
  - **Packages** — a filter sidebar (destination, price range, sort) with
    paging, and a details page with an image carousel, duration/rating/seats,
    description, approved reviews, and a booking-form UI that recalculates
    the total price live via JS as the traveler count changes (per the
    dynamic-pricing requirement) — the form doesn't submit anywhere yet,
    since `Travel.Web`'s own `BookingsController` and the wishlist button's
    AJAX call are built in Phase 7 alongside the rest of the booking system
- A few nav links (`Blog`, `Contact`, `حجوزاتي`/My Bookings) point at
  controllers that don't exist yet — Razor's `asp-controller` tag helpers
  don't validate targets at compile time, so the project still builds; those
  routes come online in the next couple of phases (public Blog/Contact
  pages, then the booking/wishlist/review UI).

### Running Travel.Web locally

`Travel.Web/appsettings.json` points `ApiSettings:BaseUrl` at
`https://localhost:7001/` — update it to match whatever port `Travel.API`
actually listens on for you (check its `launchSettings.json` / console
output), and run both projects together (e.g. multiple startup projects in
Visual Studio, or `dotnet run` in two terminals).

## Phase 6 — Admin Area (this drop)

- **New API surface for admin-only aggregate/management data** that didn't
  exist before: `GET /api/admin/dashboard` (`DashboardStatsDto` — total
  users/bookings/packages/destinations, confirmed+completed revenue,
  pending-booking count, 5 most recent bookings and users),
  `GET /api/admin/users` + `PUT /api/admin/users/{id}/lock|unlock` (backed
  by `UserManager.SetLockoutEndDateAsync`, with a guard so an Admin account
  can't be locked through this endpoint), `GET /api/packages/admin/all` and
  `GET /api/admin/reviews` (both include inactive/unapproved rows the
  public endpoints intentionally hide), and `GET /api/destinations?includeInactive=true`.
  `DestinationDto` and `PackageDto` now also carry `IsActive` so the admin
  screens can show/toggle it — the public site simply never had a reason to
  render that field before.
- **`Travel.Web/Areas/Admin`** — a full MVC Area with its own layout
  (sidebar nav, RTL, same brand styling as the public site via a shared
  `admin.css`), gated by `[Authorize(Roles = "Admin")]` on every controller,
  and its own `_ViewImports`/`_ViewStart` (areas don't inherit the root
  `Views/_ViewImports.cshtml` since `Areas/` isn't a parent directory of
  it, so it needed its own).
- **Dashboard** — the stat cards plus a recent-bookings table and
  recent-users list from `GET /api/admin/dashboard`.
- **Destinations / Packages / Blog** — full CRUD screens (Index with an
  active/inactive badge, Create, Edit). Delete is really the same
  soft-delete the service layer already enforces (`IsActive = false`),
  which is why the admin table still lists inactive rows with a "reactivate
  via Edit" path rather than a hard delete.
- **Bookings** — a table of every booking with an inline status dropdown
  wired to `PUT /api/admin/bookings/{id}/status`.
- **Reviews** — pending reviews surface first, with Approve/Delete actions.
- **Contact Messages** — inbox view with a "mark as read" action; unread
  rows render bold.
- **Users** — list with role and lockout status; Lock/Unlock actions are
  hidden for Admin accounts in the UI (and blocked server-side regardless).

### Known scope cuts in this phase

- Editing a package's image gallery (`PackageImage` rows) isn't in the admin
  UI yet — only the create form accepts a newline-separated list of image
  URLs. Managing the gallery on an existing package would need its own
  small CRUD screen; flagged rather than built silently to keep this phase
  honest about what's covered.
- `DashboardService` and `AdminUserService` query `UserManager.Users`
  (a plain `IQueryable`, no async LINQ overloads) synchronously. Fine for an
  admin-only, low-traffic screen; would need revisiting if the user table
  ever gets large enough for that to matter.

## Phase 7 — customer booking/wishlist/review flow (this drop)

- **`Travel.Web/Controllers/BookingsController`** (`[Authorize]`) — `Index`
  ("My Bookings"), `Create` (what the package-details form now actually
  submits to), `Cancel`. Booking/API errors (e.g. "only 2 seats left")
  surface via `TempData["Error"]` back on the page the person was on.
- **`Travel.Web/Controllers/WishlistController`** (`[Authorize]`) —
  `Index` ("My Wishlist"), plus `Add`/`Remove` used both as a normal form
  post (Wishlist page) and as a `fetch()` AJAX call (the heart button on
  the package details page). AJAX POSTs need the antiforgery token as a
  header rather than a form field, so `Program.cs` now sets
  `AddAntiforgery(options.HeaderName = "X-CSRF-TOKEN")` and
  `wwwroot/js/site.js` reads the token from a hidden
  `@@Html.AntiForgeryToken()` already rendered in the booking form.
- **`Travel.Web/Controllers/ReviewsController`** (`[Authorize]`) — `Create`,
  linked from a "أضف تقييماً" (Add a review) button that only appears next
  to **Completed** bookings on the My Bookings page (matching the
  `BusinessRuleException` the API already throws for anything else).
- **Package details page** now checks `User.Identity.IsAuthenticated`:
  signed-in visitors see the real booking form; anonymous visitors see a
  "log in to book" prompt that carries a `returnUrl` back to that same
  package after login, instead of silently redirecting away from a
  half-filled form.
- The public layout now renders `TempData["Success"]`/`["Error"]` as
  dismissible alerts (the Admin layout already had this from Phase 6), and
  the navbar's "حجوزاتي" (My Bookings) link — dead since Phase 5 — now
  points at the real controller, plus a new "المفضلة" (Wishlist) link.

With this phase every page and link introduced since Phase 5 is now wired
to something real; there are no more placeholder routes left in the
public site.

## Running the test suite

```bash
dotnet test Travel.Tests/Travel.Tests.csproj
```

## Running everything with Docker Compose

```bash
cp .env.example .env   # then fill in DB_SA_PASSWORD, JWT_SECRET, SEED_ADMIN_EMAIL, SEED_ADMIN_PASSWORD
docker compose up --build
```

`Travel.Web` will be on `http://localhost:8080`, `Travel.API` on
`http://localhost:8081`. Note Docker Compose here hasn't been run in this
sandbox (no Docker available) — same build-not-verified caveat as the rest
of the solution.

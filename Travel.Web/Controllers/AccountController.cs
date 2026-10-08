using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Travel.BLL.DTOs.Auth;
using Travel.Web.Services.ApiClient;
using Travel.Web.ViewModels.Account;

namespace Travel.Web.Controllers;

public class AccountController : Controller
{
    private readonly ITravelApiClient _apiClient;

    public AccountController(ITravelApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    [HttpGet]
    public IActionResult Login(string? returnUrl = null) => View(new LoginViewModel { ReturnUrl = returnUrl });

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        try
        {
            var result = await _apiClient.LoginAsync(new LoginDto { Email = model.Email, Password = model.Password });
            await SignInAsync(result);

            // An admin who isn't following a specific deep link (returnUrl)
            // lands in the admin control panel rather than the public
            // homepage -- otherwise a signed-in admin sees the exact same
            // page as everyone else and has no obvious way to find the
            // dashboard.
            if (string.IsNullOrEmpty(model.ReturnUrl) && result.Roles.Contains("Admin"))
                return RedirectToAction("Index", "Dashboard", new { area = "Admin" });

            return LocalRedirectOrHome(model.ReturnUrl);
        }
        catch (ApiException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return View(model);
        }
    }

    [HttpGet]
    public IActionResult Register() => View(new RegisterViewModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        try
        {
            var result = await _apiClient.RegisterAsync(new RegisterDto
            {
                FullName = model.FullName,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                Password = model.Password,
                ConfirmPassword = model.ConfirmPassword
            });

            await SignInAsync(result);
            return RedirectToAction("Index", "Home");
        }
        catch (ApiException ex)
        {
            foreach (var error in ex.Errors.DefaultIfEmpty(ex.Message))
                ModelState.AddModelError(string.Empty, error);

            return View(model);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Index", "Home");
    }

    public IActionResult AccessDenied() => View();

    private async Task SignInAsync(AuthResponseDto auth)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, auth.UserId),
            new(ClaimTypes.Name, auth.FullName),
            new(ClaimTypes.Email, auth.Email),
            new(JwtAuthHandler.TokenClaimType, auth.Token)
        };

        claims.AddRange(auth.Roles.Select(role => new Claim(ClaimTypes.Role, role)));

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(identity),
            new AuthenticationProperties
            {
                IsPersistent = true,
                // The cookie can't outlive the JWT it carries, since there's
                // no refresh-token flow yet -- a person is asked to log in
                // again whenever the underlying token expires (JwtSettings:
                // ExpiryMinutes on the API). Revisit if session length
                // becomes a UX problem.
                ExpiresUtc = auth.ExpiresAtUtc
            });
    }

    private IActionResult LocalRedirectOrHome(string? returnUrl) =>
        !string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl)
            ? Redirect(returnUrl)
            : RedirectToAction("Index", "Home");
}

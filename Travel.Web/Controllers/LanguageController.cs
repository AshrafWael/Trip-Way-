using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;

namespace Travel.Web.Controllers;

public class LanguageController : Controller
{
    // A plain GET link, not a POST form. Switching the display language
    // is a harmless, idempotent action -- it doesn't need CSRF
    // protection, and a simple <a href="..."> can't silently fail the
    // way a POST form can (missing/mismatched antiforgery token,
    // cookie policy quirks, JS not intercepting the submit, etc.).
    [HttpGet]
    public IActionResult SetLanguage(string culture, string? returnUrl)
    {
        Response.Cookies.Append(
            CookieRequestCultureProvider.DefaultCookieName,
            CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
            new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1), IsEssential = true });

        return LocalRedirect(!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl) ? returnUrl : "/");
    }
}

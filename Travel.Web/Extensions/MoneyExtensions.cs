using System.Globalization;

namespace Travel.Web.Extensions;

/// <summary>
/// All prices in this project are USD amounts (see the newsletter/total
/// price script in Packages/Details.cshtml, which formats with
/// currency: 'USD' explicitly). Using value.ToString("C0") on its own
/// formats with whatever currency the *current UI culture* defaults to
/// -- ar-EG defaults to Egyptian pounds -- which would silently show
/// the wrong currency symbol/format whenever a person switches the site
/// to Arabic. This keeps price formatting consistent in both languages.
/// </summary>
public static class MoneyExtensions
{
    private static readonly CultureInfo UsdCulture = CultureInfo.GetCultureInfo("en-US");

    public static string ToUsd(this decimal value) => value.ToString("C0", UsdCulture);

    /// <summary>
    /// Package/hotel prices are now optional (some trips are priced only
    /// after a WhatsApp conversation with customer service), so this
    /// returns null instead of a formatted amount when there's no price
    /// to show. Views combine it with a "contact us" fallback string,
    /// since the wording differs slightly per page.
    /// </summary>
    public static string? ToUsdOrNull(this decimal? value) => value?.ToString("C0", UsdCulture);
}

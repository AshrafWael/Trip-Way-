using System.Text.RegularExpressions;

namespace Travel.Web.Helpers;

/// <summary>
/// Booking here doesn't take payment on-site -- after a Pending
/// booking/reservation is created, the traveler continues with customer
/// service over WhatsApp to finalize dates, pricing, and payment. This
/// builds the https://wa.me/&lt;number&gt;?text=... deep link from
/// whatever the admin typed into the WhatsApp Number setting (spaces,
/// dashes, and a leading "+" are all common ways people write phone
/// numbers, so they're stripped rather than rejected).
/// </summary>
public static class WhatsAppHelper
{
    public static string? BuildChatUrl(string? whatsAppNumber, string message)
    {
        if (string.IsNullOrWhiteSpace(whatsAppNumber))
            return null;

        var digitsOnly = Regex.Replace(whatsAppNumber, @"[^\d]", "");
        if (digitsOnly.Length == 0)
            return null;

        return $"https://wa.me/{digitsOnly}?text={Uri.EscapeDataString(message)}";
    }
}

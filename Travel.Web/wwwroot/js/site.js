// Placeholder for site-wide interactivity (filters, gallery, etc.),
// filled in as those pages are built out in later phases.

// Adds a package to the signed-in person's wishlist via the Travel.Web
// proxy controller (not the API directly -- the browser only holds the
// MVC auth cookie, not the JWT, which lives server-side on that cookie's
// claims). The antiforgery token is read from the token generated on the
// page every request an @Html.AntiForgeryToken() renders, and set as a
// header per the AddAntiforgery(options.HeaderName) config in Program.cs.
function addToWishlist(packageId, antiForgeryToken, callback) {
    fetch('/Wishlist/Add?packageId=' + encodeURIComponent(packageId), {
        method: 'POST',
        headers: { 'X-CSRF-TOKEN': antiForgeryToken }
    })
        .then(response => response.json())
        .then(result => callback(result))
        .catch(() => callback({ success: false, message: 'تعذر الاتصال بالخادم.' }));
}

// Subscribes an email to the newsletter via the Travel.Web proxy
// controller, which in turn calls POST /api/newsletter/subscribe.
function subscribeToNewsletter(email, antiForgeryToken, callback) {
    const body = new URLSearchParams({ email });
    fetch('/Newsletter/Subscribe', {
        method: 'POST',
        headers: {
            'X-CSRF-TOKEN': antiForgeryToken,
            'Content-Type': 'application/x-www-form-urlencoded'
        },
        body
    })
        .then(response => response.json())
        .then(result => callback(result))
        .catch(() => callback({ success: false, message: 'تعذر الاتصال بالخادم.' }));
}

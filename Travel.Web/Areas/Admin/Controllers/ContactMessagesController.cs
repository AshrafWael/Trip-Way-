using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Travel.Web.Services.ApiClient;

namespace Travel.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class ContactMessagesController : Controller
{
    private readonly ITravelApiClient _apiClient;

    public ContactMessagesController(ITravelApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public async Task<IActionResult> Index()
    {
        var messages = await _apiClient.GetContactMessagesAsync();
        return View(messages);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> MarkAsRead(int id)
    {
        await _apiClient.MarkContactMessageReadAsync(id);
        return RedirectToAction(nameof(Index));
    }
}

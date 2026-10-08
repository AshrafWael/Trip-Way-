using Microsoft.AspNetCore.Mvc;
using Travel.Web.Services.ApiClient;

namespace Travel.Web.Controllers;

public class DestinationsController : Controller
{
    private readonly ITravelApiClient _apiClient;

    public DestinationsController(ITravelApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public async Task<IActionResult> Index()
    {
        var destinations = await _apiClient.GetDestinationsAsync();
        return View(destinations);
    }

    public async Task<IActionResult> Details(int id)
    {
        try
        {
            var destination = await _apiClient.GetDestinationAsync(id);
            return View(destination);
        }
        catch (ApiException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return NotFound();
        }
    }
}

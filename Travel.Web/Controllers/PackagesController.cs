using Microsoft.AspNetCore.Mvc;
using Travel.BLL.DTOs.Packages;
using Travel.Web.Services.ApiClient;
using Travel.Web.ViewModels.Packages;

namespace Travel.Web.Controllers;

public class PackagesController : Controller
{
    private readonly ITravelApiClient _apiClient;

    public PackagesController(ITravelApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public async Task<IActionResult> Index([FromQuery] PackageSearchParamsDto searchParams)
    {
        var result = await _apiClient.SearchPackagesAsync(searchParams);
        var destinations = await _apiClient.GetDestinationsAsync();

        return View(new PackageListViewModel
        {
            Result = result,
            Destinations = destinations,
            SearchParams = searchParams
        });
    }

    public async Task<IActionResult> Details(int id)
    {
        try
        {
            var package = await _apiClient.GetPackageAsync(id);

            try
            {
                ViewBag.WhatsAppNumber = (await _apiClient.GetSiteSettingsAsync()).WhatsAppNumber;
            }
            catch (ApiException)
            {
                ViewBag.WhatsAppNumber = null;
            }

            return View(package);
        }
        catch (ApiException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return NotFound();
        }
    }
}

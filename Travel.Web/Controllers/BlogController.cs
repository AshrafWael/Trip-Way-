using Microsoft.AspNetCore.Mvc;
using Travel.Web.Services.ApiClient;

namespace Travel.Web.Controllers;

public class BlogController : Controller
{
    private readonly ITravelApiClient _apiClient;

    public BlogController(ITravelApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public async Task<IActionResult> Index()
    {
        var posts = await _apiClient.GetBlogPostsAsync();
        return View(posts);
    }

    public async Task<IActionResult> Details(int id)
    {
        try
        {
            var post = await _apiClient.GetBlogPostAsync(id);
            return View(post);
        }
        catch (ApiException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return NotFound();
        }
    }
}

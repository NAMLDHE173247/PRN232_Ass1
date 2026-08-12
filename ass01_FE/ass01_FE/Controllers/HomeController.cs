using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using ass01_FE.Services;
using ass01_FE.Models.News;
using System.Dynamic;

namespace ass01_FE.Controllers;

public class HomeController : Controller
{
    private readonly NewsApiService _newsApiService;

    public HomeController(NewsApiService newsApiService)
    {
        _newsApiService = newsApiService;
    }

    public async Task<IActionResult> Index(string? keyword, int page = 1)
    {
        ViewBag.Keyword = keyword;
        int pageSize = 5;

        // Fetch all active news. OData is supported on BE but for simplicity in FE we fetch and paginate.
        var allNews = await _newsApiService.GetActiveNewsAsync(keyword);

        // Simple Pagination
        var totalItems = allNews.Count;
        var totalPages = (int)System.Math.Ceiling(totalItems / (double)pageSize);
        
        if (page < 1) page = 1;
        if (page > totalPages && totalPages > 0) page = totalPages;

        var pagedNews = allNews.Skip((page - 1) * pageSize).Take(pageSize).ToList();

        ViewBag.CurrentPage = page;
        ViewBag.TotalPages = totalPages;

        return View(pagedNews);
    }

    public async Task<IActionResult> Detail(string id)
    {
        var article = await _newsApiService.GetNewsByIdAsync(id);
        if (article == null)
        {
            return NotFound();
        }

        var related = await _newsApiService.GetRelatedNewsAsync(id);

        dynamic model = new ExpandoObject();
        model.Article = article;
        model.Related = related;

        return View(model);
    }
}

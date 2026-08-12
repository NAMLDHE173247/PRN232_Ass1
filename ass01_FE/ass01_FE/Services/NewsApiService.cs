using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using ass01_FE.Models.News;

namespace ass01_FE.Services;

public class NewsApiService
{
    private readonly HttpClient _httpClient;

    public NewsApiService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _httpClient.BaseAddress = new System.Uri(configuration["ApiSettings:BaseUrl"] ?? "https://localhost:7087/");
    }

    public async Task<List<NewsArticleDto>> GetActiveNewsAsync(string? keyword = null)
    {
        var url = "api/news";
        if (!string.IsNullOrEmpty(keyword))
        {
            url += $"?keyword={System.Uri.EscapeDataString(keyword)}";
        }

        var response = await _httpClient.GetAsync(url);
        if (response.IsSuccessStatusCode)
        {
            var articles = await response.Content.ReadFromJsonAsync<List<NewsArticleDto>>();
            return articles ?? new List<NewsArticleDto>();
        }
        return new List<NewsArticleDto>();
    }

    public async Task<NewsArticleDto?> GetNewsByIdAsync(string id)
    {
        var response = await _httpClient.GetAsync($"api/news/{id}");
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<NewsArticleDto>();
        }
        return null;
    }

    public async Task<List<NewsArticleDto>> GetRelatedNewsAsync(string id)
    {
        var response = await _httpClient.GetAsync($"api/news/{id}/related");
        if (response.IsSuccessStatusCode)
        {
            var articles = await response.Content.ReadFromJsonAsync<List<NewsArticleDto>>();
            return articles ?? new List<NewsArticleDto>();
        }
        return new List<NewsArticleDto>();
    }
}

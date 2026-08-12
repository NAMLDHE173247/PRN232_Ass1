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
        var baseUrl = configuration["ApiSettings:BaseUrl"] ?? throw new System.InvalidOperationException("ApiSettings:BaseUrl is not configured.");
        _httpClient.BaseAddress = new System.Uri(baseUrl);
    }

    public async Task<(List<NewsArticleDto> Items, int TotalCount)> GetActiveNewsAsync(string? keyword = null, int skip = 0, int top = 5)
    {
        var url = $"api/news?$skip={skip}&$top={top}&$count=true";
        if (!string.IsNullOrEmpty(keyword))
        {
            url += $"&keyword={System.Uri.EscapeDataString(keyword)}";
        }

        var response = await _httpClient.GetAsync(url);
        if (response.IsSuccessStatusCode)
        {
            try
            {
                var odataResult = await response.Content.ReadFromJsonAsync<ODataResponse<NewsArticleDto>>();
                if (odataResult != null && odataResult.Value != null)
                {
                    return (odataResult.Value, odataResult.Count);
                }
            }
            catch
            {
                // Fallback if backend does not wrap in OData format (e.g. returns plain array)
                var listResult = await response.Content.ReadFromJsonAsync<List<NewsArticleDto>>();
                if (listResult != null)
                {
                    return (listResult, listResult.Count);
                }
            }
        }
        return (new List<NewsArticleDto>(), 0);
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

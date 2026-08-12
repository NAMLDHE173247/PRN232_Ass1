using System.Collections.Generic;
using System.Threading.Tasks;
using ass01.BusinessLogic.DTOs.NewsArticle;

namespace ass01.BusinessLogic.Services;

public interface INewsArticleService
{
    Task<List<NewsArticleDto>> GetNewsArticlesAsync();
    Task<NewsArticleDto?> GetNewsArticleByIdAsync(string id);
    Task<NewsArticleDto> CreateNewsArticleAsync(CreateNewsArticleRequest request, short currentUserId);
    Task UpdateNewsArticleAsync(string id, UpdateNewsArticleRequest request, short currentUserId);
    Task DeleteNewsArticleAsync(string id);
    Task<List<NewsArticleDto>> GetRelatedNewsArticlesAsync(string id);
}

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ass01.Models;

namespace ass01.DataAccess.DAOs;

public interface INewsArticleDAO
{
    Task<List<NewsArticle>> GetNewsArticlesAsync();
    Task<NewsArticle?> GetNewsArticleByIdAsync(string id);
    Task AddNewsArticleAsync(NewsArticle article);
    Task UpdateNewsArticleAsync(NewsArticle article);
    Task DeleteNewsArticleAsync(NewsArticle article);
    Task<bool> IsDuplicateTitleWithin24HoursAsync(string title, DateTime currentTime, string? excludeId = null);
    Task<List<NewsArticle>> GetRelatedNewsArticlesAsync(string articleId, short? categoryId, List<int> tagIds);
}

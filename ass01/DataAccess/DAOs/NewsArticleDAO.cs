using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ass01.Models;
using Microsoft.EntityFrameworkCore;

namespace ass01.DataAccess.DAOs;

public class NewsArticleDAO : INewsArticleDAO
{
    private readonly FunewsManagementContext _context;

    public NewsArticleDAO(FunewsManagementContext context)
    {
        _context = context;
    }

    public async Task<List<NewsArticle>> GetNewsArticlesAsync()
    {
        return await _context.NewsArticles
            .Include(n => n.Category)
            .Include(n => n.Tags)
            .Include(n => n.CreatedBy)
            .ToListAsync();
    }

    public async Task<NewsArticle?> GetNewsArticleByIdAsync(string id)
    {
        return await _context.NewsArticles
            .Include(n => n.Category)
            .Include(n => n.Tags)
            .Include(n => n.CreatedBy)
            .FirstOrDefaultAsync(n => n.NewsArticleId == id);
    }

    public async Task AddNewsArticleAsync(NewsArticle article)
    {
        await _context.NewsArticles.AddAsync(article);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateNewsArticleAsync(NewsArticle article)
    {
        _context.NewsArticles.Update(article);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteNewsArticleAsync(NewsArticle article)
    {
        _context.NewsArticles.Remove(article);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> IsDuplicateTitleWithin24HoursAsync(string title, DateTime currentTime, string? excludeId = null)
    {
        var cutoff = currentTime.AddHours(-24);
        var query = _context.NewsArticles
            .Where(n => n.NewsTitle == title && n.CreatedDate >= cutoff);

        if (!string.IsNullOrEmpty(excludeId))
        {
            query = query.Where(n => n.NewsArticleId != excludeId);
        }

        return await query.AnyAsync();
    }

    public async Task<List<NewsArticle>> GetRelatedNewsArticlesAsync(string articleId, short? categoryId, List<int> tagIds)
    {
        var query = _context.NewsArticles
            .Include(n => n.Category)
            .Include(n => n.Tags)
            .Where(n => n.NewsArticleId != articleId && n.NewsStatus == true);

        return await query
            .Where(n => (categoryId.HasValue && n.CategoryId == categoryId.Value) || 
                        n.Tags.Any(t => tagIds.Contains(t.TagId)))
            .OrderByDescending(n => n.CreatedDate)
            .Take(3)
            .ToListAsync();
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ass01.BusinessLogic.DTOs.NewsArticle;
using ass01.BusinessLogic.DTOs.Tag;
using ass01.DataAccess.Repositories;
using ass01.Models;

namespace ass01.BusinessLogic.Services;

public class NewsArticleService : INewsArticleService
{
    private readonly INewsArticleRepository _newsRepo;
    private readonly ITagRepository _tagRepo;

    public NewsArticleService(INewsArticleRepository newsRepo, ITagRepository tagRepo)
    {
        _newsRepo = newsRepo;
        _tagRepo = tagRepo;
    }

    public async Task<List<NewsArticleDto>> GetNewsArticlesAsync(bool isStaff)
    {
        var articles = await _newsRepo.GetNewsArticlesAsync(isStaff);
        return articles.Select(MapToDto).ToList();
    }

    public async Task<NewsArticleDto?> GetNewsArticleByIdAsync(string id, bool isStaff)
    {
        var article = await _newsRepo.GetNewsArticleByIdAsync(id, isStaff);
        if (article == null) return null;
        return MapToDto(article);
    }

    public async Task<NewsArticleDto> CreateNewsArticleAsync(CreateNewsArticleRequest request, short currentUserId)
    {
        if (await _newsRepo.IsDuplicateTitleWithin24HoursAsync(request.NewsTitle, DateTime.UtcNow))
            throw new ArgumentException("A news article with this title was already created within the last 24 hours.");

        var newArticleId = DateTime.UtcNow.Ticks.ToString();

        var article = new NewsArticle
        {
            NewsArticleId = newArticleId,
            NewsTitle = request.NewsTitle,
            Headline = request.Headline,
            NewsContent = request.NewsContent,
            NewsSource = request.NewsSource,
            CategoryId = request.CategoryId,
            NewsStatus = request.NewsStatus,
            CreatedById = currentUserId,
            CreatedDate = DateTime.UtcNow
        };

        if (request.TagIds != null && request.TagIds.Any())
        {
            var allTags = await _tagRepo.GetTagsAsync();
            var validTags = allTags.Where(t => request.TagIds.Contains(t.TagId)).ToList();
            foreach (var tag in validTags)
            {
                article.Tags.Add(tag);
            }
        }

        await _newsRepo.AddNewsArticleAsync(article);

        // Fetch again to get fully loaded navigational properties
        var created = await _newsRepo.GetNewsArticleByIdAsync(newArticleId, true);
        return MapToDto(created!);
    }

    public async Task UpdateNewsArticleAsync(string id, UpdateNewsArticleRequest request, short currentUserId)
    {
        var article = await _newsRepo.GetNewsArticleByIdAsync(id, true);
        if (article == null)
            throw new KeyNotFoundException("NewsArticle not found.");

        if (await _newsRepo.IsDuplicateTitleWithin24HoursAsync(request.NewsTitle, DateTime.UtcNow, id))
            throw new ArgumentException("A news article with this title was already created within the last 24 hours.");

        article.NewsTitle = request.NewsTitle;
        article.Headline = request.Headline;
        article.NewsContent = request.NewsContent;
        article.NewsSource = request.NewsSource;
        article.CategoryId = request.CategoryId;
        article.NewsStatus = request.NewsStatus;
        article.UpdatedById = currentUserId;
        article.ModifiedDate = DateTime.UtcNow;

        if (request.TagIds != null)
        {
            var allTags = await _tagRepo.GetTagsAsync();
            var newTags = allTags.Where(t => request.TagIds.Contains(t.TagId)).ToList();
            
            article.Tags.Clear();
            foreach (var t in newTags)
            {
                article.Tags.Add(t);
            }
        }

        await _newsRepo.UpdateNewsArticleAsync(article);
    }

    public async Task DeleteNewsArticleAsync(string id)
    {
        var article = await _newsRepo.GetNewsArticleByIdAsync(id, true);
        if (article == null)
            throw new KeyNotFoundException("NewsArticle not found.");

        // Need to clear tags first before deleting to satisfy FK constraints if cascade isn't working as expected,
        // though EF Core handles cascade/ClientSetNull in memory if loaded. Let's explicitly clear.
        article.Tags.Clear();
        await _newsRepo.UpdateNewsArticleAsync(article);

        await _newsRepo.DeleteNewsArticleAsync(article);
    }

    public async Task<List<NewsArticleDto>> GetRelatedNewsArticlesAsync(string id)
    {
        var article = await _newsRepo.GetNewsArticleByIdAsync(id, true);
        if (article == null)
            throw new KeyNotFoundException("NewsArticle not found.");

        var tagIds = article.Tags.Select(t => t.TagId).ToList();
        
        var related = await _newsRepo.GetRelatedNewsArticlesAsync(id, article.CategoryId, tagIds);

        return related.Select(MapToDto).ToList();
    }

    private static NewsArticleDto MapToDto(NewsArticle a)
    {
        return new NewsArticleDto
        {
            NewsArticleId = a.NewsArticleId,
            NewsTitle = a.NewsTitle,
            Headline = a.Headline,
            CreatedDate = a.CreatedDate,
            NewsContent = a.NewsContent,
            NewsSource = a.NewsSource,
            CategoryId = a.CategoryId,
            CategoryName = a.Category?.CategoryName,
            NewsStatus = a.NewsStatus,
            CreatedById = a.CreatedById,
            CreatedByName = a.CreatedBy?.AccountName,
            UpdatedById = a.UpdatedById,
            ModifiedDate = a.ModifiedDate,
            Tags = a.Tags.Select(t => new TagDto
            {
                TagId = t.TagId,
                TagName = t.TagName,
                Note = t.Note
            }).ToList()
        };
    }
}

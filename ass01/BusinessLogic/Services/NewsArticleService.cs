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

    public async Task<List<NewsArticleDto>> GetNewsArticlesAsync(bool isStaff, string? keyword = null, short? categoryId = null, string? tagName = null, short? createdById = null, DateTime? startDate = null, DateTime? endDate = null, string? authorName = null, bool? newsStatus = null)
    {
        var articles = await _newsRepo.GetNewsArticlesAsync(isStaff);
        var query = articles.AsQueryable();

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            var lowerKeyword = keyword.ToLower();
            query = query.Where(a => 
                (a.NewsTitle != null && a.NewsTitle.ToLower().Contains(lowerKeyword)) ||
                (a.Headline != null && a.Headline.ToLower().Contains(lowerKeyword)) ||
                (a.NewsContent != null && a.NewsContent.ToLower().Contains(lowerKeyword))
            );
        }

        if (categoryId.HasValue)
        {
            query = query.Where(a => a.CategoryId == categoryId.Value);
        }

        if (!string.IsNullOrWhiteSpace(tagName))
        {
            var lowerTag = tagName.ToLower();
            query = query.Where(a => a.Tags.Any(t => t.TagName != null && t.TagName.ToLower().Contains(lowerTag)));
        }

        if (createdById.HasValue)
        {
            query = query.Where(a => a.CreatedById == createdById.Value);
        }

        if (startDate.HasValue)
        {
            query = query.Where(a => a.CreatedDate >= startDate.Value);
        }

        if (endDate.HasValue)
        {
            var endOfDay = endDate.Value.Date.AddDays(1);
            query = query.Where(a => a.CreatedDate < endOfDay);
        }

        if (!string.IsNullOrWhiteSpace(authorName))
        {
            var lowerAuthor = authorName.ToLower();
            query = query.Where(a => a.CreatedBy != null && a.CreatedBy.AccountName != null && a.CreatedBy.AccountName.ToLower().Contains(lowerAuthor));
        }

        if (newsStatus.HasValue)
        {
            query = query.Where(a => a.NewsStatus == newsStatus.Value);
        }

        return query.Select(MapToDto).ToList();
    }

    public async Task<NewsArticleDto?> GetNewsArticleByIdAsync(string id, bool isStaff)
    {
        var article = await _newsRepo.GetNewsArticleByIdAsync(id, isStaff);
        if (article == null) return null;
        return MapToDto(article);
    }

    public async Task<NewsArticleDto> CreateNewsArticleAsync(CreateNewsArticleRequest request, short currentUserId)
    {
        var newArticleId = DateTime.Now.Ticks.ToString();
        while (await _newsRepo.GetNewsArticleByIdAsync(newArticleId, true) != null)
        {
            newArticleId = DateTime.Now.Ticks.ToString();
        }

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
            CreatedDate = DateTime.Now
        };

        if (request.TagIds != null && request.TagIds.Any())
        {
            var allTags = await _tagRepo.GetTagsAsync();
            var validTags = allTags.Where(t => request.TagIds.Contains(t.TagId)).ToList();
            if (validTags.Count != request.TagIds.Distinct().Count())
            {
                throw new ArgumentException("One or more tags do not exist.");
            }
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

        article.NewsTitle = request.NewsTitle;
        article.Headline = request.Headline;
        article.NewsContent = request.NewsContent;
        article.NewsSource = request.NewsSource;
        article.CategoryId = request.CategoryId;
        article.NewsStatus = request.NewsStatus;
        article.UpdatedById = currentUserId;
        article.ModifiedDate = DateTime.Now;

        if (request.TagIds != null)
        {
            var allTags = await _tagRepo.GetTagsAsync();
            var newTags = allTags.Where(t => request.TagIds.Contains(t.TagId)).ToList();
            
            if (newTags.Count != request.TagIds.Distinct().Count())
            {
                throw new ArgumentException("One or more tags do not exist.");
            }

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

        article.Tags.Clear();
        await _newsRepo.UpdateNewsArticleAsync(article);

        await _newsRepo.DeleteNewsArticleAsync(article);
    }

    public async Task<NewsArticleDto> DuplicateNewsArticleAsync(string id, short currentUserId)
    {
        var original = await _newsRepo.GetNewsArticleByIdAsync(id, true);
        if (original == null)
            throw new KeyNotFoundException("Original NewsArticle not found.");

        var newArticleId = DateTime.Now.Ticks.ToString();
        while (await _newsRepo.GetNewsArticleByIdAsync(newArticleId, true) != null)
        {
            newArticleId = DateTime.Now.Ticks.ToString();
        }

        var duplicated = new NewsArticle
        {
            NewsArticleId = newArticleId,
            NewsTitle = original.NewsTitle,
            Headline = original.Headline,
            NewsContent = original.NewsContent,
            NewsSource = original.NewsSource,
            CategoryId = original.CategoryId,
            NewsStatus = original.NewsStatus,
            CreatedById = currentUserId,
            CreatedDate = DateTime.Now
        };

        foreach (var tag in original.Tags)
        {
            duplicated.Tags.Add(tag);
        }

        await _newsRepo.AddNewsArticleAsync(duplicated);

        var created = await _newsRepo.GetNewsArticleByIdAsync(newArticleId, true);
        return MapToDto(created!);
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
            UpdatedByName = a.UpdatedBy?.AccountName,
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

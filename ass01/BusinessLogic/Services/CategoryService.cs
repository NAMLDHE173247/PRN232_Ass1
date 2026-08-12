using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ass01.BusinessLogic.DTOs.Category;
using ass01.DataAccess.Repositories;
using ass01.Models;

namespace ass01.BusinessLogic.Services;

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _repository;

    public CategoryService(ICategoryRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<CategoryDto>> GetCategoriesAsync(string? searchKeyword)
    {
        var categories = await _repository.GetCategoriesAsync();
        var query = categories.AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchKeyword))
        {
            var keyword = searchKeyword.ToLower();
            query = query.Where(c => 
                (c.CategoryName != null && c.CategoryName.ToLower().Contains(keyword)) ||
                (c.CategoryDesciption != null && c.CategoryDesciption.ToLower().Contains(keyword))
            );
        }

        return query.Select(c => new CategoryDto
        {
            CategoryId = c.CategoryId,
            CategoryName = c.CategoryName,
            CategoryDescription = c.CategoryDesciption ?? string.Empty,
            ParentCategoryId = c.ParentCategoryId,
            IsActive = c.IsActive,
            ArticleCount = c.NewsArticles.Count
        }).ToList();
    }

    public async Task<CategoryDto?> GetCategoryByIdAsync(short id)
    {
        var c = await _repository.GetCategoryByIdAsync(id);
        if (c == null) return null;

        return new CategoryDto
        {
            CategoryId = c.CategoryId,
            CategoryName = c.CategoryName,
            CategoryDescription = c.CategoryDesciption ?? string.Empty,
            ParentCategoryId = c.ParentCategoryId,
            IsActive = c.IsActive,
            ArticleCount = c.NewsArticles?.Count ?? 0
        };
    }

    public async Task<CategoryDto> CreateCategoryAsync(CreateCategoryRequest request)
    {
        if (await _repository.CategoryNameExistsAsync(request.CategoryName, request.ParentCategoryId))
        {
            throw new ArgumentException("A category with this name already exists under the specified parent category.");
        }

        var category = new Category
        {
            CategoryName = request.CategoryName,
            CategoryDesciption = request.CategoryDescription,
            ParentCategoryId = request.ParentCategoryId,
            IsActive = request.IsActive
        };

        await _repository.AddCategoryAsync(category);

        return new CategoryDto
        {
            CategoryId = category.CategoryId,
            CategoryName = category.CategoryName,
            CategoryDescription = category.CategoryDesciption ?? string.Empty,
            ParentCategoryId = category.ParentCategoryId,
            IsActive = category.IsActive
        };
    }

    public async Task UpdateCategoryAsync(short id, UpdateCategoryRequest request)
    {
        var category = await _repository.GetCategoryByIdAsync(id);
        if (category == null)
            throw new KeyNotFoundException("Category not found.");

        if (request.ParentCategoryId == id)
            throw new ArgumentException("Parent category cannot be the same as the current category.");

        if (category.ParentCategoryId != request.ParentCategoryId)
        {
            var hasNews = await _repository.HasNewsArticlesAsync(id);
            if (hasNews)
            {
                throw new InvalidOperationException("Cannot change parent category because this category is being used by news articles.");
            }
        }

        if (await _repository.CategoryNameExistsAsync(request.CategoryName, request.ParentCategoryId, id))
        {
            throw new ArgumentException("A category with this name already exists under the specified parent category.");
        }

        category.CategoryName = request.CategoryName;
        category.CategoryDesciption = request.CategoryDescription;
        category.ParentCategoryId = request.ParentCategoryId;
        category.IsActive = request.IsActive;

        await _repository.UpdateCategoryAsync(category);
    }

    public async Task DeleteCategoryAsync(short id)
    {
        var category = await _repository.GetCategoryByIdAsync(id);
        if (category == null)
            throw new KeyNotFoundException("Category not found.");

        var hasNews = await _repository.HasNewsArticlesAsync(id);
        if (hasNews)
            throw new InvalidOperationException("Cannot delete this category because it is linked to news articles.");

        await _repository.DeleteCategoryAsync(category);
    }
}

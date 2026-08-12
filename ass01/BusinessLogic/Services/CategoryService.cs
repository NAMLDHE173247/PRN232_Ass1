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

    public async Task<List<CategoryDto>> GetCategoriesAsync()
    {
        var categories = await _repository.GetCategoriesAsync();
        return categories.Select(c => new CategoryDto
        {
            CategoryId = c.CategoryId,
            CategoryName = c.CategoryName,
            CategoryDescription = c.CategoryDesciption ?? string.Empty, // Note: DB typo mapped correctly
            ParentCategoryId = c.ParentCategoryId,
            IsActive = c.IsActive
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
            IsActive = c.IsActive
        };
    }

    public async Task<CategoryDto> CreateCategoryAsync(CreateCategoryRequest request)
    {
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

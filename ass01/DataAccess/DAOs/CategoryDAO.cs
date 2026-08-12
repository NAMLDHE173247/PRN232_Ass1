using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using ass01.Models;
using Microsoft.EntityFrameworkCore;

namespace ass01.DataAccess.DAOs;

public class CategoryDAO : ICategoryDAO
{
    private readonly FunewsManagementContext _context;

    public CategoryDAO(FunewsManagementContext context)
    {
        _context = context;
    }

    public async Task<List<Category>> GetCategoriesAsync()
    {
        return await _context.Categories
            .Include(c => c.NewsArticles)
            .ToListAsync();
    }

    public async Task<Category?> GetCategoryByIdAsync(short id)
    {
        return await _context.Categories
            .Include(c => c.NewsArticles)
            .FirstOrDefaultAsync(c => c.CategoryId == id);
    }

    public async Task AddCategoryAsync(Category category)
    {
        await _context.Categories.AddAsync(category);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateCategoryAsync(Category category)
    {
        _context.Categories.Update(category);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteCategoryAsync(Category category)
    {
        _context.Categories.Remove(category);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> HasNewsArticlesAsync(short categoryId)
    {
        return await _context.NewsArticles
            .AnyAsync(n => n.CategoryId == categoryId);
    }

    public async Task<bool> CategoryNameExistsAsync(string categoryName, short? parentCategoryId, short? excludeCategoryId = null)
    {
        var query = _context.Categories.Where(c => c.CategoryName == categoryName && c.ParentCategoryId == parentCategoryId);
        if (excludeCategoryId.HasValue)
        {
            query = query.Where(c => c.CategoryId != excludeCategoryId.Value);
        }
        return await query.AnyAsync();
    }
}

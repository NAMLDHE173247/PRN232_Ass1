using System.Collections.Generic;
using System.Threading.Tasks;
using ass01.BusinessLogic.DTOs.Category;

namespace ass01.BusinessLogic.Services;

public interface ICategoryService
{
    Task<List<CategoryDto>> GetCategoriesAsync(string? searchKeyword);
    Task<CategoryDto?> GetCategoryByIdAsync(short id);
    Task<CategoryDto> CreateCategoryAsync(CreateCategoryRequest request);
    Task UpdateCategoryAsync(short id, UpdateCategoryRequest request);
    Task DeleteCategoryAsync(short id);
}

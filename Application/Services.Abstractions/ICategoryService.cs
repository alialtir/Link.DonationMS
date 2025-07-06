using DTOs.CategoryDTOs;

namespace Application.Services.Abstractions
{
    public interface ICategoryService
    {
        Task<CategoryDto> GetByIdAsync(int id);
        Task<IEnumerable<CategoryDto>> GetAllAsync();
        Task<CategoryDto> CreateAsync(CreateCategoryDto createCategoryDto);
        Task<CategoryDto> UpdateAsync(int id, UpdateCategoryDto updateCategoryDto);
        Task<bool> DeleteAsync(int id);
    }
} 
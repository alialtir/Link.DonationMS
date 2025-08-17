using Application.Services.Abstractions;
using DTOs.CategoryDTOs;
using AutoMapper;
using Domain.Contracts;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using static Services.Specifications.CategorySpecifications;

namespace Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CategoryService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<CategoryDto> CreateAsync(CreateCategoryDto createCategoryDto)
        {
            var category = _mapper.Map<Category>(createCategoryDto);
            await _unitOfWork.Categories.AddAsync(category);
            await _unitOfWork.CompleteAsync();
            return _mapper.Map<CategoryDto>(category);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var category = await _unitOfWork.Categories.GetByIdAsync(id);
            if (category == null) return false;
            _unitOfWork.Categories.Delete(category);
            await _unitOfWork.CompleteAsync();
            return true;
        }

        public async Task<IEnumerable<CategoryDto>> GetAllAsync(int pageNumber = 1, int pageSize = 5)
        {
            if (pageSize <= 0) pageSize = 5;
            var spec = new CategoriesWithPaginationSpecification(pageNumber, pageSize);
            var categories = await _unitOfWork.Categories.ListAsync(spec);
            return _mapper.Map<IEnumerable<CategoryDto>>(categories);
        }

        public async Task<CategoryDto> GetByIdAsync(int id)
        {
            var category = await _unitOfWork.Categories.GetByIdAsync(id);
            if (category == null) return null;
            return _mapper.Map<CategoryDto>(category);
        }

        public async Task<CategoryDto> UpdateAsync(int id, UpdateCategoryDto updateCategoryDto)
        {
            var category = await _unitOfWork.Categories.GetByIdAsync(id);
            if (category == null) return null;
            _mapper.Map(updateCategoryDto, category);
            _unitOfWork.Categories.Update(category);
            await _unitOfWork.CompleteAsync();
            return _mapper.Map<CategoryDto>(category);
        }

        public async Task<int> GetCountAsync()
        {
            var spec = new AllCategoriesSpecification();
            return await _unitOfWork.Categories.CountAsync(spec);
        }
    }
} 
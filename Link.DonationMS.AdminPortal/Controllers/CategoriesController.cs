using DTOs.CategoryDTOs;
using Microsoft.AspNetCore.Mvc;
using Link.DonationMS.AdminPortal.Models;

namespace Link.DonationMS.AdminPortal.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly ApiService _apiService;

        public CategoriesController(ApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<IActionResult> Index()
        {
            var categories = await _apiService.GetCategoriesAsync();
            return View(categories);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateCategoryDto dto)
        {
            if (ModelState.IsValid)
            {
                var result = await _apiService.CreateCategoryAsync(dto);
                if (result != null)
                {
                    TempData["Success"] = "Category created successfully.";
                    return RedirectToAction(nameof(Index));
                }
                ModelState.AddModelError("", "Failed to create category. Please check your input and try again.");
            }
            return View(dto);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var category = await _apiService.GetCategoryByIdAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, CategoryDto dto)
        {
            if (ModelState.IsValid)
            {
                var updateDto = new UpdateCategoryDto
                {
                    TitleAr = dto.TitleAr,
                    TitleEn = dto.TitleEn,
                    DescriptionAr = dto.DescriptionAr,
                    DescriptionEn = dto.DescriptionEn
                };

                var result = await _apiService.UpdateCategoryAsync(id, updateDto);
                if (result != null)
                {
                    TempData["Success"] = "Category updated successfully.";
                    return RedirectToAction(nameof(Index));
                }
                ModelState.AddModelError("", "Failed to update category. Please check your input and try again.");
            }
            return View(dto);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _apiService.DeleteCategoryAsync(id);
            if (result)
            {
                TempData["Success"] = "Category deleted successfully.";
            }
            else
            {
                TempData["Error"] = "Failed to delete category. Please try again.";
            }
            return RedirectToAction(nameof(Index));
        }
    }
} 
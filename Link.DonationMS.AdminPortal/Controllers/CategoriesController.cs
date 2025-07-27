using DTOs.CategoryDTOs;
using Microsoft.AspNetCore.Mvc;
using Link.DonationMS.AdminPortal.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;

namespace Link.DonationMS.AdminPortal.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CategoriesController : Controller
    {
        private readonly ApiService _apiService;
        private readonly IConfiguration _configuration;

        public CategoriesController(ApiService apiService, IConfiguration configuration)
        {
            _apiService = apiService;
            _configuration = configuration;
        }

        public async Task<IActionResult> Index(int page = 1)
        {
            var categories = await _apiService.GetCategoriesAsync(page);
            var totalCount = await _apiService.GetCategoriesCountAsync();
            var pageSize = _configuration.GetValue<int>("PageSize", 5);
            
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalCount / pageSize);
            ViewBag.HasPreviousPage = page > 1;
            ViewBag.HasNextPage = page < ViewBag.TotalPages && categories.Any();
            
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
            
            var updateDto = new UpdateCategoryDto
            {
                TitleAr = category.TitleAr,
                TitleEn = category.TitleEn,
                DescriptionAr = category.DescriptionAr,
                DescriptionEn = category.DescriptionEn
            };
            
            return View(updateDto);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, UpdateCategoryDto dto)
        {
            if (ModelState.IsValid)
            {
                var result = await _apiService.UpdateCategoryAsync(id, dto);
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
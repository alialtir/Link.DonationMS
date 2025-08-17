using Link.DonationMS.AdminPortal.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using DTOs.CampaignDTOs;
using System.IO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;

namespace Link.DonationMS.AdminPortal.Controllers
{
    [Authorize(Roles = "Admin,CampaignManager")]
    public class CampaignsController : Controller
    {
        private readonly ApiService _apiService;
        private readonly IConfiguration _configuration;
        
        public CampaignsController(ApiService apiService, IConfiguration configuration)
        {
            _apiService = apiService;
            _configuration = configuration;
        }

        public async Task<IActionResult> Index(int page = 1)
        {
            var campaigns = await _apiService.GetCampaignsAsync(page);
            var totalCount = await _apiService.GetCampaignsCountAsync();
            var pageSize = _configuration.GetValue<int>("PageSize", 5);
            
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalCount / pageSize);
            ViewBag.HasPreviousPage = page > 1;
            ViewBag.HasNextPage = page < ViewBag.TotalPages && campaigns.Any();
            
            return View(campaigns);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            try
            {
                var categories = await _apiService.GetCategoriesAsync();
                ViewBag.Categories = new SelectList(categories, "Id", "TitleAr");
            }
            catch
            {
                ViewBag.Categories = new SelectList(Enumerable.Empty<object>());
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateCampaignDto dto, IFormFile? imageFile)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (imageFile != null && imageFile.Length > 0)
                    {
                        using var memoryStream = new MemoryStream();
                        await imageFile.CopyToAsync(memoryStream);
                        dto.ImageData = memoryStream.ToArray();
                        dto.ImageExtension = Path.GetExtension(imageFile.FileName).TrimStart('.');
                    }
                    
                    await _apiService.CreateCampaignAsync(dto);
                    TempData["Success"] = "Campaign created successfully.";
                    return RedirectToAction("Index");
                }
                catch (HttpRequestException ex)
                {
                    ModelState.AddModelError("CategoryId", "Please select a valid category.");
                    TempData["Error"] = "Failed to create campaign. Please check your input and try again.";
                }
            }
            
            try
            {
                var categories = await _apiService.GetCategoriesAsync();
                ViewBag.Categories = new SelectList(categories, "Id", "TitleAr");
            }
            catch
            {
                ViewBag.Categories = new SelectList(Enumerable.Empty<object>());
            }
            
            return View(dto);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id, int page = 1)
        {
            var campaign = await _apiService.GetCampaignByIdAsync(id);
            if (campaign == null) return NotFound();
            
            var updateDto = new UpdateCampaignDto
            {
                TitleAr = campaign.TitleAr,
                TitleEn = campaign.TitleEn,
                DescriptionAr = campaign.DescriptionAr,
                DescriptionEn = campaign.DescriptionEn,
                GoalAmount = campaign.GoalAmount,
                EndDate = campaign.EndDate,
                CategoryId = campaign.CategoryId,
                ImageData = campaign.ImageData,
                ImageExtension = campaign.ImageExtension,
                Status = campaign.Status
            };
            ViewBag.CurrentPage = page;
            try
            {
                var categories = await _apiService.GetCategoriesAsync();
                ViewBag.Categories = new SelectList(categories, "Id", "TitleAr");
            }
            catch
            {
                ViewBag.Categories = new SelectList(Enumerable.Empty<object>());
            }
            
            return View(updateDto);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, UpdateCampaignDto dto, IFormFile? imageFile, int page = 1)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (imageFile != null && imageFile.Length > 0)
                    {
                        using var memoryStream = new MemoryStream();
                        await imageFile.CopyToAsync(memoryStream);
                        dto.ImageData = memoryStream.ToArray();
                        dto.ImageExtension = Path.GetExtension(imageFile.FileName).TrimStart('.');
                    }
                    
                    // Normalize decimal separator to dot before sending JSON
                    dto.GoalAmount = decimal.Parse(dto.GoalAmount.ToString().Replace(',', '.'), System.Globalization.CultureInfo.InvariantCulture);
                    await _apiService.UpdateCampaignAsync(id, dto);
                    TempData["Success"] = "Campaign updated successfully.";
                    return RedirectToAction("Index", new { page = page });
                }
                catch (Exception ex)
                {
                    TempData["Error"] = $"Update failed: {ex.Message}";
                }
            }
            
            try
            {
                var categories = await _apiService.GetCategoriesAsync();
                ViewBag.Categories = new SelectList(categories, "Id", "TitleAr");
            }
            catch
            {
                ViewBag.Categories = new SelectList(Enumerable.Empty<object>());
            }
            ViewBag.CurrentPage = page;
            if (!ModelState.IsValid)
            {
                var modelErrors = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                TempData["Error"] = modelErrors;
            }
            return View(dto);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id, int page = 1)
        {
            try
            {
                await _apiService.DeleteCampaignAsync(id);
                TempData["Success"] = "Campaign deleted successfully.";
            }
            catch
            {
                TempData["Error"] = "Failed to delete campaign. Please try again.";
            }
            return RedirectToAction("Index", new { page = page });
        }
    }
}

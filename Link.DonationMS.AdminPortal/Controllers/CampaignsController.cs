using Link.DonationMS.AdminPortal.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using DTOs.CampaignDTOs;
using System.IO;

namespace Link.DonationMS.AdminPortal.Controllers
{
    public class CampaignsController : Controller
    {
        private readonly ApiService _apiService;
        public CampaignsController(ApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<IActionResult> Index(int page = 1)
        {
            var campaigns = await _apiService.GetCampaignsAsync(page);
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
        public async Task<IActionResult> Edit(int id)
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
                ImageExtension = campaign.ImageExtension
            };
            
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
        public async Task<IActionResult> Edit(int id, UpdateCampaignDto dto, IFormFile? imageFile)
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
                    
                    await _apiService.UpdateCampaignAsync(id, dto);
                    TempData["Success"] = "Campaign updated successfully.";
                    return RedirectToAction("Index");
                }
                catch
                {
                    TempData["Error"] = "Failed to update campaign. Please check your input and try again.";
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

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
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
            return RedirectToAction("Index");
        }
    }
} 
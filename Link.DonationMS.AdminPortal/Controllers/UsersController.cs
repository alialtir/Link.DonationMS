using DTOs.UserDTOs;
using Microsoft.AspNetCore.Mvc;
using Link.DonationMS.AdminPortal.Models;

namespace Link.DonationMS.AdminPortal.Controllers
{
    public class UsersController : Controller
    {
        private readonly ApiService _apiService;
        public UsersController(ApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<IActionResult> Donors()
        {
            var donors = await _apiService.GetDonorsAsync();
            return View(donors);
        }

        public async Task<IActionResult> Admins()
        {
            var admins = await _apiService.GetAdminsAsync();
            return View(admins);
        }

        public IActionResult CreateAdmin()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateAdmin(RegisterUserDto dto)
        {
            if (ModelState.IsValid)
            {
                var result = await _apiService.CreateAdminAsync(dto);
                if (result != null)
                {
                    TempData["Success"] = "Admin user created successfully.";
                    return RedirectToAction("Admins");
                }
                ModelState.AddModelError("", "An error occurred while creating the admin user.");
            }
            return View(dto);
        }

        public async Task<IActionResult> EditAdmin(Guid id)
        {
            var admin = await _apiService.GetAdminByIdAsync(id);
            if (admin == null) return NotFound();
            
            if (admin.Email == "admin@linkdonation.com")
            {
                TempData["Error"] = "The original system administrator cannot be modified.";
                return RedirectToAction("Admins");
            }
            
            return View(admin);
        }

        [HttpPost]
        public async Task<IActionResult> EditAdmin(Guid id, UserDto dto)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var result = await _apiService.UpdateAdminAsync(id, dto);
                    if (result != null)
                    {
                        TempData["Success"] = "Admin user updated successfully.";
                        return RedirectToAction("Admins");
                    }
                }
                catch (InvalidOperationException ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
                catch
                {
                    ModelState.AddModelError("", "An error occurred while updating the admin user.");
                }
            }
            return View(dto);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteAdmin(Guid id)
        {
            try
            {
                await _apiService.DeleteAdminAsync(id);
                TempData["Success"] = "Admin user deleted successfully.";
            }
            catch (InvalidOperationException ex)
            {
                TempData["Error"] = ex.Message;
            }
            catch
            {
                TempData["Error"] = "An error occurred while deleting the admin user.";
            }
            return RedirectToAction("Admins");
        }
    }
} 
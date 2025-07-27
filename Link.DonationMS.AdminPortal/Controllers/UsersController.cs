using DTOs.UserDTOs;
using Microsoft.AspNetCore.Mvc;
using Link.DonationMS.AdminPortal.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;

namespace Link.DonationMS.AdminPortal.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UsersController : Controller
    {
        private readonly ApiService _apiService;
        private readonly IConfiguration _configuration;
        
        public UsersController(ApiService apiService, IConfiguration configuration)
        {
            _apiService = apiService;
            _configuration = configuration;
        }

        public async Task<IActionResult> Users(int page = 1)
        {
            var users = await _apiService.GetDonorsAsync();
            var pageSize = _configuration.GetValue<int>("PageSize", 5);
            
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling((double)users.Count() / pageSize);
            ViewBag.HasPreviousPage = page > 1;
            ViewBag.HasNextPage = page < ViewBag.TotalPages && users.Any();
            
            return View(users);
        }

        public async Task<IActionResult> Donors(int page = 1)
        {
            var donors = await _apiService.GetDonorsAsync();
            var pageSize = _configuration.GetValue<int>("PageSize", 5);
            
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling((double)donors.Count() / pageSize);
            ViewBag.HasPreviousPage = page > 1;
            ViewBag.HasNextPage = page < ViewBag.TotalPages && donors.Any();
            
            return View(donors);
        }

        public async Task<IActionResult> Admins(int page = 1)
        {
            var admins = await _apiService.GetAdminsAsync();
            var pageSize = _configuration.GetValue<int>("PageSize", 5);
            
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling((double)admins.Count() / pageSize);
            ViewBag.HasPreviousPage = page > 1;
            ViewBag.HasNextPage = page < ViewBag.TotalPages && admins.Any();
            
            return View(admins);
        }

        public IActionResult CreateAdmin()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateAdmin(CreateAdminDto dto)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    UserDto result;
                    string successMessage;
                    string redirectAction;

                    result = await _apiService.CreateAdminAsync(dto);
                    successMessage = "Admin user created successfully.";
                    redirectAction = "Admins";

                    if (result != null)
                    {
                        TempData["Success"] = successMessage;
                        return RedirectToAction(redirectAction);
                    }
                }
                catch (InvalidOperationException ex)
                {
                    if (ex.Message.Contains("Username") && ex.Message.Contains("Email"))
                    {
                        ModelState.AddModelError("Email", "Email is already taken");
                    }
                    else if (ex.Message.Contains("Username"))
                    {
                        ModelState.AddModelError("Email", "Username is already taken");
                    }
                    else if (ex.Message.Contains("Email"))
                    {
                        ModelState.AddModelError("Email", "Email is already taken");
                    }
                    else
                    {
                        ModelState.AddModelError("", ex.Message);
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "An error occurred while creating the user");
                }
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
            dto.Id = id;
            
            if (ModelState.IsValid)
            {
                try
                {
                    var result = await _apiService.UpdateAdminAsync(id, dto);
                    if (result != null)
                    {
                        TempData["Success"] = "User updated successfully.";
                        return RedirectToAction("Admins");
                    }
                }
                catch (InvalidOperationException ex)
                {
                    if (ex.Message.Contains("Username") && ex.Message.Contains("Email"))
                    {
                        ModelState.AddModelError("Email", "Email is already taken");
                    }
                    else if (ex.Message.Contains("Username"))
                    {
                        ModelState.AddModelError("Email", "Username is already taken");
                    }
                    else if (ex.Message.Contains("Email"))
                    {
                        ModelState.AddModelError("Email", "Email is already taken");
                    }
                    else
                    {
                        ModelState.AddModelError("", ex.Message);
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "An error occurred while updating the user");
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
                TempData["Success"] = "User deleted successfully.";
            }
            catch (InvalidOperationException ex)
            {
                TempData["Error"] = ex.Message;
            }
            catch
            {
                TempData["Error"] = "An error occurred while deleting the user.";
            }
            
            return RedirectToAction("Admins");
        }
    }
} 
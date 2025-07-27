using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Application.Dtos.UserDTOs;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using System.Linq;
using System.Collections.Generic;
using Link.DonationMS.AdminPortal.Models;

namespace Link.DonationMS.AdminPortal.Controllers
{
    public class AuthController : Controller
    {
        private readonly ApiService _apiService;
        public AuthController(ApiService apiService)
        {
            _apiService = apiService;
        }

        [HttpGet]
        public async Task<IActionResult> Login()
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                if (!User.IsInRole("Admin"))
                {
                    await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                    Response.Cookies.Delete("AccessToken");
                    ModelState.AddModelError("", "Access denied. Only Administrators can access Admin Portal.");
                    return View();
                }
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginDto model)
        {
            try
            {
                var result = await _apiService.LoginAsync(model);
                if (result == null || !result.Succeeded)
                {
                    ModelState.AddModelError("", result?.Error ?? "Login failed");
                    return View(model);
                }
                if (result.Roles == null || !result.Roles.Contains("Admin"))
                {
                    ModelState.AddModelError("", "غير مسموح لك بالدخول");
                    return View(model);
                }
                if (result.RequiresPasswordReset)
                {
                    return RedirectToAction("ResetPassword", new { email = model.UserName });
                }
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, result.UserId ?? ""),
                    new Claim(ClaimTypes.Name, result.UserName ?? "")
                };
                if (result.Roles != null)
                    claims.AddRange(result.Roles.Select(r => new Claim(ClaimTypes.Role, r)));
                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
                Response.Cookies.Append("AccessToken", result.AccessToken ?? "");
                return RedirectToAction("Index", "Home");
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(model);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"An unexpected error occurred: {ex.Message}");
                return View(model);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            Response.Cookies.Delete("AccessToken");
            return RedirectToAction("Login", "Auth");
        }

        [HttpGet]
        public IActionResult ResetPassword(string email)
        {
            ViewBag.Email = email;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(string email, string newPassword, string confirmPassword)
        {
            if (newPassword != confirmPassword)
            {
                TempData["Error"] = "Passwords do not match.";
                ViewBag.Email = email;
                return View();
            }
            var result = await _apiService.ResetPasswordByEmailAsync(email, newPassword);
            if (result)
            {
                return RedirectToAction("Login");
            }
            TempData["Error"] = "An error occurred while changing the password.";
            ViewBag.Email = email;
            return View();
        }
    }
} 
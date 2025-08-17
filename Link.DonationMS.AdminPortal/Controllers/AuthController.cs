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
    [Route("[controller]")]
    public class AuthController : Controller
    {
        private readonly ApiService _apiService;
        private readonly ILogger<AuthController> _logger;
        public AuthController(ApiService apiService, ILogger<AuthController> logger)
        {
            _apiService = apiService;
            _logger = logger;
        }

        [HttpGet("Login")]
        public async Task<IActionResult> Login()
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                if (!User.IsInRole("Admin") && !User.IsInRole("CampaignManager"))
                {
                    await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                    Response.Cookies.Delete("AccessToken");
                    ModelState.AddModelError("", "Access denied. Only Administrators and Campaign Managers can access Admin Portal.");
                    return View();
                }
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost("Login")]
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
                // Debug: Log the roles to see what we're getting
                var rolesDebug = result.Roles != null ? string.Join(", ", result.Roles) : "No roles";
                _logger?.LogInformation($"User {model.UserName} has roles: {rolesDebug}");
                
                if (result.Roles == null || (!result.Roles.Contains("Admin") && !result.Roles.Contains("CampaignManager")))
                {
                    ModelState.AddModelError("", $"You are not authorized to access this area. Only Administrators and Campaign Managers are allowed. Your roles: {rolesDebug}");
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

        [HttpPost("Logout")]
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

        [HttpGet("login-google")]
        public IActionResult LoginGoogle(string returnUrl = "")
        {
            var redirectUrl = Url.Action("GoogleCallback", "Auth", new { returnUrl });
            var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
            return Challenge(properties, "Google");
        }

        [HttpGet("GoogleCallback")]
        public async Task<IActionResult> GoogleCallback()
        {
            var authResult = await HttpContext.AuthenticateAsync("Google");
            if (!authResult.Succeeded || authResult.Principal == null)
            {
                TempData["Error"] = "Google authentication failed";
                return RedirectToAction("Login");
            }

            var idToken = authResult.Properties?.GetTokenValue("id_token");
            if (string.IsNullOrEmpty(idToken))
            {
                TempData["Error"] = "Failed to retrieve Google token.";
                return RedirectToAction("Login");
            }

            var apiResult = await _apiService.GoogleLoginAsync(idToken);
            if (apiResult == null || !apiResult.Succeeded)
            {
                TempData["Error"] = apiResult?.Error ?? "Login failed";
                return RedirectToAction("Login");
            }
            if (apiResult.Roles == null || !apiResult.Roles.Contains("Admin"))
            {
                TempData["Error"] = "You are not authorized to access this area";
                return RedirectToAction("Login");
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, apiResult.UserId ?? string.Empty),
                new Claim(ClaimTypes.Name, apiResult.UserName ?? string.Empty)
            };
            if (apiResult.Roles != null)
                claims.AddRange(apiResult.Roles.Select(r => new Claim(ClaimTypes.Role, r)));

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));
            Response.Cookies.Append("AccessToken", apiResult.AccessToken ?? string.Empty);

            //if (!string.IsNullOrEmpty(returnUrl))
            //    return Redirect(returnUrl);
            return RedirectToAction("Index", "Home");
        }

    }
} 
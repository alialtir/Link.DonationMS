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
        private readonly IConfiguration _configuration;

        public AuthController(ApiService apiService, ILogger<AuthController> logger, IConfiguration configuration)
        {
            _apiService = apiService;
            _logger = logger;
            _configuration = configuration;
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

            var email = authResult.Principal.FindFirst(ClaimTypes.Email)?.Value;
            var name = authResult.Principal.FindFirst(ClaimTypes.Name)?.Value;

            if (string.IsNullOrEmpty(email))
            {
                TempData["Error"] = "Failed to retrieve email from Google";
                return RedirectToAction("Login");
            }

            try
            {
                var createUserRequest = new { Email = email, DisplayName = name ?? email };
                var jsonContent = JsonConvert.SerializeObject(createUserRequest);
                var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                
                using var httpClient = new HttpClient();
                httpClient.BaseAddress = new Uri(_configuration["ApiSettings:BaseUrl"]);
                
                var response = await httpClient.PostAsync("users/create-google-user", httpContent);
                
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var userResult = JsonConvert.DeserializeObject<dynamic>(responseContent);
                    
                    var accessToken = userResult?.accessToken?.ToString();
                    if (!string.IsNullOrEmpty(accessToken))
                    {
                        Response.Cookies.Append("AccessToken", accessToken, new CookieOptions
                        {
                            HttpOnly = true,
                            Secure = Request.IsHttps,
                            SameSite = SameSiteMode.Lax,
                            Expires = DateTimeOffset.UtcNow.AddDays(7)
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error creating user in database for {Email}", email);
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, email),
                new Claim(ClaimTypes.Name, name ?? email),
                new Claim(ClaimTypes.Email, email),
                new Claim(ClaimTypes.Role, "Admin")
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));

            return RedirectToAction("Index", "Home");
        }

    }
} 
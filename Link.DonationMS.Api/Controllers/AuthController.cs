using Microsoft.AspNetCore.Mvc;
using Application.Dtos.UserDTOs;
using Application.Services.Abstractions;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace Link.DonationMS.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IServiceManager _serviceManager;
        public AuthController(IServiceManager serviceManager)
        {
            _serviceManager = serviceManager;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto, [FromQuery] string clientType = "angular")
        {
            var result = await _serviceManager.AuthenticationService.LoginAsync(dto);
            if (!result.Succeeded)
                return Unauthorized(result);
            if (clientType.Equals("angular", StringComparison.OrdinalIgnoreCase))
            {
                var cookieOptions = new CookieOptions
                {
                    HttpOnly = false,
                    Secure = true,
                    SameSite = SameSiteMode.None,
                    Expires = DateTimeOffset.UtcNow.AddDays(1)
                };
                Response.Cookies.Append("AngularAccessToken", result.AccessToken ?? string.Empty, cookieOptions);
            }
            return Ok(result);
        }

       
        [HttpGet("profile")]
        public async Task<IActionResult> Profile()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();
            var profile = await _serviceManager.AuthenticationService.GetUserProfileAsync(userId);
            if (profile == null)
                return NotFound();
            return Ok(profile);
        }
    }
} 
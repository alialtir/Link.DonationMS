using Microsoft.AspNetCore.Mvc;
using Application.Dtos.UserDTOs;
using Application.Services.Abstractions;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

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
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var result = await _serviceManager.AuthenticationService.LoginAsync(dto);
            if (!result.Succeeded)
                return Unauthorized(result);
            return Ok(result);
        }

        [Authorize]
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
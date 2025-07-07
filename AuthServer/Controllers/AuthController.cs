using Application.Dtos.UserDTOs;
using Application.Services.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AuthServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthenticationService _authenticationService;

        public AuthController(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _authenticationService.LoginAsync(loginDto);

            if (!result.Succeeded)
            {
                return Unauthorized(new { error = result.Error });
            }

            return Ok(new
            {
                access_token = result.AccessToken,
                user_id = result.UserId,
                user_name = result.UserName,
                email = result.Email,
                roles = result.Roles
            });
        }

        [HttpGet("profile")]
        [Authorize]
        public async Task<IActionResult> GetProfile()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var result = await _authenticationService.GetUserProfileAsync(userId);

            if (!result.Succeeded)
            {
                return NotFound(new { error = result.Error });
            }

            return Ok(new
            {
                user_id = result.UserId,
                user_name = result.UserName,
                email = result.Email,
                first_name = result.FirstName,
                last_name = result.LastName,
                roles = result.Roles
            });
        }

        [HttpGet("validate")]
        [Authorize]
        public IActionResult ValidateToken()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userName = User.FindFirst(ClaimTypes.Name)?.Value;
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            var roles = User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();

            return Ok(new
            {
                user_id = userId,
                user_name = userName,
                email = email,
                roles = roles,
                is_valid = true
            });
        }
    }
} 
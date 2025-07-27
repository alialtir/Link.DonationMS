using Application.Services.Abstractions;
using DTOs.UserDTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace Link.DonationMS.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class UsersController : ControllerBase
    {
        private readonly IServiceManager _serviceManager;
        public UsersController(IServiceManager serviceManager)
        {
            _serviceManager = serviceManager;
        }

        [HttpGet("donors")]
        public async Task<IActionResult> GetDonors()
        {
            var allUsers = await _serviceManager.UserService.GetAllAsync();
            var donorsOnly = new List<UserDto>();
            
            foreach (var user in allUsers)
            {
                var roles = await _serviceManager.UserService.GetUserRolesAsync(user.Id);
                if (roles.Contains("Donor") && !roles.Contains("Admin") && !roles.Contains("CampaignManager"))
                {
                    donorsOnly.Add(user);
                }
            }
            
            return Ok(donorsOnly);
        }

        [HttpGet("admins")]
        public async Task<IActionResult> GetAdmins()
        {
            var admins = await _serviceManager.UserService.GetUsersInRoleAsync("Admin");
            var managers = await _serviceManager.UserService.GetUsersInRoleAsync("CampaignManager");
            var all = admins.Concat(managers).ToList();
            return Ok(all);
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserDto dto)
        {
            try
            {
                var user = await _serviceManager.UserService.RegisterAsync(dto);
                return Ok(user);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost("admins")]
        public async Task<IActionResult> CreateAdmin([FromBody] CreateAdminDto dto)
        {
            try
            {
                var registerDto = new RegisterUserDto
                {
                    DisplayName = dto.DisplayName,
                    Email = dto.Email,
                    Password = dto.Password,
                    ConfirmPassword = dto.Password 
                };
                
                var user = await _serviceManager.UserService.RegisterAsync(registerDto);
                
                
                await _serviceManager.UserService.RemoveFromRoleAsync(user.Id, "Donor");
                await _serviceManager.UserService.AddToRoleAsync(user.Id, dto.Role);
                
                
                user.Role = dto.Role;
                
                return Ok(user);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPut("admins/{id}")]
        public async Task<IActionResult> UpdateAdmin(Guid id, [FromBody] UserDto dto)
        {
            try
            {
                var user = await _serviceManager.UserService.UpdateAsync(id, dto);
                if (user == null) return NotFound();
                return Ok(user);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpDelete("admins/{id}")]
        public async Task<IActionResult> DeleteAdmin(Guid id)
        {
            try
            {
                var result = await _serviceManager.UserService.DeleteAsync(id);
                if (!result) return NotFound();
                return Ok();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [AllowAnonymous]
        [HttpPost("reset-password-by-email")]
        public async Task<IActionResult> ResetPasswordByEmail([FromBody] ResetPasswordByEmailDto dto)
        {
            var result = await _serviceManager.UserService.ResetPasswordByEmailAsync(dto.Email, dto.NewPassword);
            if (!result)
                return BadRequest();
            return Ok();
        }
    }
} 
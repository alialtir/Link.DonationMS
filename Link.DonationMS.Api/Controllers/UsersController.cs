using Application.Services.Abstractions;
using DTOs.UserDTOs;
using Application.Dtos.UserDTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Domain.Models;

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
                    DisplayNameAr = dto.DisplayName, // Using the same name for both languages for now
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

        [HttpPost("reset-password-by-email")]
        public async Task<IActionResult> ResetPasswordByEmail([FromBody] ResetPasswordByEmailDto resetPasswordDto)
        {
            var result = await _serviceManager.UserService.ResetPasswordByEmailAsync(resetPasswordDto.Email, resetPasswordDto.NewPassword);
            if (!result)
                return BadRequest();
            return Ok();
        }

        [AllowAnonymous]
        [HttpPost("create-google-user")]
        public async Task<IActionResult> CreateGoogleUser([FromBody] CreateGoogleUserDto createUserDto)
        {
            try
            {
                var existingUsers = await _serviceManager.UserService.GetAllAsync();
                var existingUser = existingUsers.FirstOrDefault(u => u.Email == createUserDto.Email);
                
                if (existingUser == null)
                {
                    var registerDto = new RegisterUserDto
                    {
                        DisplayName = createUserDto.DisplayName,
                        DisplayNameAr = createUserDto.DisplayName,
                        Email = createUserDto.Email,
                        Password = "TempPassword123!",
                        ConfirmPassword = "TempPassword123!"
                    };

                    existingUser = await _serviceManager.UserService.RegisterAsync(registerDto);
                    await _serviceManager.UserService.RemoveFromRoleAsync(existingUser.Id, "Donor");
                }

                var userRoles = await _serviceManager.UserService.GetUserRolesAsync(existingUser.Id);
                if (!userRoles.Contains("Admin"))
                {
                    await _serviceManager.UserService.AddToRoleAsync(existingUser.Id, "Admin");
                }

                var mockUser = new Domain.Models.User 
                { 
                    Id = existingUser.Id, 
                    Email = existingUser.Email,
                    UserName = existingUser.Email,
                    DisplayName = existingUser.DisplayName ?? existingUser.Email
                };
                
                var jwt = await _serviceManager.AuthenticationService.GenerateJwtToken(mockUser);

                return Ok(new 
                { 
                    userId = existingUser.Id,
                    email = existingUser.Email,
                    accessToken = jwt
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }
} 
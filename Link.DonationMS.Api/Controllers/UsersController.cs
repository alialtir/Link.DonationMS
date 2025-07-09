using Application.Services.Abstractions;
using DTOs.UserDTOs;
using Microsoft.AspNetCore.Mvc;

namespace Link.DonationMS.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
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
            var result = await _serviceManager.UserService.GetUsersInRoleAsync("Donor");
            return Ok(result);
        }

        [HttpGet("admins")]
        public async Task<IActionResult> GetAdmins()
        {
            var result = await _serviceManager.UserService.GetUsersInRoleAsync("Admin");
            return Ok(result);
        }

        [HttpPost("admins")]
        public async Task<IActionResult> CreateAdmin([FromBody] RegisterUserDto dto)
        {
            var user = await _serviceManager.UserService.RegisterAsync(dto);
            await _serviceManager.UserService.AddToRoleAsync(user.Id, "Admin");
            return Ok(user);
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
    }
} 
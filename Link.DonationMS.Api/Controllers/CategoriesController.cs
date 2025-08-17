using Application.Services.Abstractions;
using DTOs.CategoryDTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace Link.DonationMS.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class CategoriesController : ControllerBase
    {
        private readonly IServiceManager _serviceManager;
        public CategoriesController(IServiceManager serviceManager)
        {
            _serviceManager = serviceManager;
        }

       
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 5)
        {
            var result = await _serviceManager.CategoryService.GetAllAsync(page, pageSize);
            return Ok(result);
        }

        [HttpGet("count")]
        public async Task<IActionResult> GetCount()
        {
            var count = await _serviceManager.CategoryService.GetCountAsync();
            return Ok(count);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _serviceManager.CategoryService.GetByIdAsync(id);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCategoryDto dto)
        {
            var result = await _serviceManager.CategoryService.CreateAsync(dto);
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateCategoryDto dto)
        {
            var result = await _serviceManager.CategoryService.UpdateAsync(id, dto);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _serviceManager.CategoryService.DeleteAsync(id);
            if (!result) return NotFound();
            return Ok();
        }
    }
} 
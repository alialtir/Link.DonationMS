using Application.Services.Abstractions;
using DTOs.CampaignDTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace Link.DonationMS.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class CampaignsController : ControllerBase
    {
        private readonly IServiceManager _serviceManager;
        public CampaignsController(IServiceManager serviceManager)
        {
            _serviceManager = serviceManager;
        }


        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 6)
        {
            var items = await _serviceManager.CampaignService.GetAllAsync(page, pageSize);
            var totalCount = await _serviceManager.CampaignService.GetCountAsync();
            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);
            return Ok(new {
                items,
                totalCount,
                totalPages,
                page,
                pageSize
            });
        }

    
        [HttpGet("count")]
        public async Task<IActionResult> GetCount()
        {
            var count = await _serviceManager.CampaignService.GetCountAsync();
            return Ok(count);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _serviceManager.CampaignService.GetByIdAsync(id);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCampaignDto dto)
        {
            var result = await _serviceManager.CampaignService.CreateAsync(dto);
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateCampaignDto dto)
        {
            var result = await _serviceManager.CampaignService.UpdateAsync(id, dto);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _serviceManager.CampaignService.DeleteAsync(id);
            if (!result) return NotFound();
            return Ok();
        }

        [AllowAnonymous]
        [HttpGet("active")]
        public async Task<IActionResult> GetActive([FromQuery] string? title, [FromQuery] int? categoryId, [FromQuery] int page = 1, [FromQuery] int pageSize = 5)
        {
            var result = await _serviceManager.CampaignService.GetActiveCampaignsFilteredAsync(title, categoryId, page, pageSize);
            return Ok(result);
        }

        [AllowAnonymous]
        [HttpGet("completed")]
        public async Task<IActionResult> GetCompleted()
        {
            var result = await _serviceManager.CampaignService.GetCompletedCampaignsAsync();
            return Ok(result);
        }

    }
} 
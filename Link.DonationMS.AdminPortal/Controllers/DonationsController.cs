using Link.DonationMS.AdminPortal.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace Link.DonationMS.AdminPortal.Controllers
{
    [Authorize(Roles = "Admin")]
    public class DonationsController : Controller
    {
        private readonly ApiService _apiService;
        public DonationsController(ApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<IActionResult> Index(int page = 1)
        {
            var donations = await _apiService.GetDonationsAsync(page);
            return View(donations);
        }
    }
} 
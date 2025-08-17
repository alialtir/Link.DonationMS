using Link.DonationMS.AdminPortal.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

[Authorize(Roles = "Admin,CampaignManager")]
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ApiService _apiService;

    public HomeController(ILogger<HomeController> logger, ApiService apiService)
    {
        _logger = logger;
        _apiService = apiService;
    }

    public IActionResult Index()
    {
        if (!User.IsInRole("Admin") && !User.IsInRole("CampaignManager"))
        {
            return RedirectToAction("Login", "Auth");
        }
        
        // Campaign Managers go directly to Campaigns, Admins go to Dashboard
        if (User.IsInRole("CampaignManager") && !User.IsInRole("Admin"))
        {
            return RedirectToAction("Index", "Campaigns");
        }
        
        // Only Admins go to Dashboard
        if (User.IsInRole("Admin"))
        {
            return RedirectToAction("Dashboard", "Home");
        }
        
        // Fallback - should not reach here, but redirect to login if no valid role
        return RedirectToAction("Login", "Auth");
    }

    public async Task<IActionResult> Dashboard([FromQuery] int? count = null)
    {
        // Dashboard is only for Admins
        if (!User.IsInRole("Admin"))
        {
            // If user is logged in but not authorized, show unauthorized page
            if (User.Identity.IsAuthenticated)
                return RedirectToAction("Unauthorized", "Home");
            else
                return RedirectToAction("Login", "Auth");
        }
        var overview = await _apiService.GetDashboardOverviewAsync();
        var topCampaigns = (await _apiService.GetTopCampaignsAsync(count)).ToList();
        var vm = new Link.DonationMS.AdminPortal.Models.ViewModels.DashboardViewModel
        {
            Overview = overview,
            TopCampaigns = topCampaigns
        };
        return View(vm);
    }

    [AllowAnonymous]
    public IActionResult Unauthorized()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}

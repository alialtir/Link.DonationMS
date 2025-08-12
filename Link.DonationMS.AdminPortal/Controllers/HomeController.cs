using Link.DonationMS.AdminPortal.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

[Authorize]
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
        if (!User.IsInRole("Admin"))
        {
            return RedirectToAction("Login", "Auth");
        }
        return RedirectToAction("Dashboard", "Home");
    }

    public async Task<IActionResult> Dashboard()
    {
        if (!User.IsInRole("Admin"))
            return RedirectToAction("Login", "Auth");

        var overview = await _apiService.GetDashboardOverviewAsync();
        var topCampaigns = (await _apiService.GetTopCampaignsAsync()).ToList();
        var vm = new Link.DonationMS.AdminPortal.Models.ViewModels.DashboardViewModel
        {
            Overview = overview,
            TopCampaigns = topCampaigns
        };
        return View(vm);
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

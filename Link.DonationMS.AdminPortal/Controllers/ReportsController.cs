using Microsoft.AspNetCore.Mvc;

namespace Link.DonationMS.AdminPortal.Controllers
{
    public class ReportsController : Controller
    {
        private readonly IConfiguration _configuration;
        public ReportsController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // Displays Donor History report using SSRS iframe
        public IActionResult DonorHistory()
        {
            var serverUrl = _configuration["ReportSettings:ServerUrl"] ?? string.Empty;
            var reportPath = _configuration["ReportSettings:DonorHistoryPath"] ?? string.Empty;

            if (string.IsNullOrWhiteSpace(serverUrl) || string.IsNullOrWhiteSpace(reportPath))
            {
                return Content("Report server configuration is missing. Please set ReportSettings in appsettings.json.");
            }

            // Encode report path and append to the server URL
            string fullUrl = $"{serverUrl}?%2f{reportPath.TrimStart('/').Replace("/", "%2f")}&rs:Command=Render&rs:Embed=true";

            ViewBag.ReportUrl = fullUrl;
            return View();
        }

    }
}

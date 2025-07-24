using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OIF.Cams.Model.ViewModels;
using PolicyGenie.Models;
using System.Diagnostics;

namespace PolicyGenie.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            var userName = HttpContext.Session.GetString("UserName");
            var activeRole = HttpContext.Session.GetString("ActiveRole");

            if (User.Identity.IsAuthenticated && (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(activeRole)))
            {
                return RedirectToAction("SilentLogout", "Account");
            }
            var header = new LayoutHeaderViewModel
            {
                //LogoUrl = "/Image/BhartiAXA_images.png",
                BranchAddress = "Dubai",
                AppLogoUrl = "/Image/PolicyGenieLogo.png"
            };

            //HttpContext.Session.SetString("LogoUrl", header.LogoUrl);
            HttpContext.Session.SetString("BranchAddress", header.BranchAddress);
            HttpContext.Session.SetString("AppLogoUrl", header.AppLogoUrl);
            if (activeRole == "Agent")
            {
                return RedirectToAction("ServiceRequestDashboard", "ServiceRequest", new { dashboardType = "TotalTask" });
            }
            else
            {
                return RedirectToAction("ServiceRequestDashboard", "ServiceRequest", new { dashboardType = "Submitted" });
            }
        }

        [Authorize]
        public IActionResult Privacy()
        {
            return View();
        }
        [Authorize(Roles = "Admin")]
        public IActionResult Admin()
        {
            return View();
        }
        [Authorize(Roles = "Agent")]
        public IActionResult Users()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

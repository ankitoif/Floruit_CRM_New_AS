using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using OIF.Cams.Business.AutoMapping.ServiceRequest;
using OIF.Cams.Data.DAC;
using OIF.Cams.Data.DAC.AppDbContext;
using OIF.Cams.Model.LeadVM;
using OIF.Cams.Model.ViewModels;
using System.Data;
using System.Text;

namespace PolicyGenie.Controllers
{
    [Authorize]
    public class ServiceRequestController : Controller
    {
        private readonly IServiceRequestAutoMapper iServiceRequestAutoMapper;
        private readonly IWebHostEnvironment hostingEnvironment;

        public ServiceRequestController(IServiceRequestAutoMapper _iServiceRequestAutoMapper, IWebHostEnvironment _hostingEnvironment)
        {
            iServiceRequestAutoMapper = _iServiceRequestAutoMapper;
            hostingEnvironment = _hostingEnvironment;
        }

        public async Task<IActionResult> CreateServiceRequest()
        {
            //var model = await iServiceRequestAutoMapper.GetCreateServiceRequestModelAsync();
            LeadViewModel model = new LeadViewModel();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> CreateServiceRequest(ServiceRequestViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model = await iServiceRequestAutoMapper.GetCreateServiceRequestModelAsync();
                return View(model);
            }

            var uploadPath = Path.Combine(hostingEnvironment.WebRootPath, "uploads");
            var currentUser = User.Identity.Name;

            var documentFilePairs = new List<ServiceRequestDocument>();

          

            foreach (var file in Request.Form.Files)
            {
                var fieldName = file.Name; 
                if (fieldName.StartsWith("Documents_") && long.TryParse(fieldName.Split('_')[1], out long docId))
                {
                    documentFilePairs.Add(new ServiceRequestDocument
                    {
                        File = file,
                        ServiceTypeDocumentId = docId
                    });
                }
            }

            var result = await iServiceRequestAutoMapper.SaveServiceRequestAsync(model, documentFilePairs, uploadPath, currentUser);

            if (!result)
            {
                ModelState.AddModelError("", "Failed to save request.");
                model = await iServiceRequestAutoMapper.GetCreateServiceRequestModelAsync();
                return View(model);
            }

            TempData["SuccessMessage"] = "Service request Created successfully.";
            return RedirectToAction("ServiceRequestDashboard", new { dashboardType = "TotalTask" });
        }



        public async Task<IActionResult> ServiceRequestDashboard(string dashboardType)
        {
            var currentUser = User.Identity.Name;
            //var isAdmin = User.IsInRole("Admin");
            var activeRole = HttpContext.Session.GetString("ActiveRole");
            ViewBag.ActiveRole = activeRole;
            bool isAdmin = activeRole != "Agent";
            var model = new ServiceRequestDashboardViewModel();

            model = await iServiceRequestAutoMapper.GetServiceRequestDashboardAsync(dashboardType, currentUser, isAdmin);               
            model.DashboardType = dashboardType;
            return View(model);
        }

        public async Task<IActionResult> DownloadServiceRequestReport(string dashboardType)
        {
            var currentUser = User.Identity.Name;
            var activeRole = HttpContext.Session.GetString("ActiveRole");
            bool isAdmin = activeRole == "Admin";

            var fileBytes = await iServiceRequestAutoMapper.GetServiceRequestReportExcelAsync(dashboardType, currentUser, isAdmin);
            var fileName = $"ServiceReport_{dashboardType}_{DateTime.Now:yyyyMMdd}.xlsx";
            return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }
        public async Task<IActionResult> DownloadReportSection()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> GenerateReport(DateTime fromDate, DateTime toDate, string reportTitle, string reportFormat)
        {
            var currentUser = User.Identity.Name;
            var isAdmin = HttpContext.Session.GetString("ActiveRole") == "Admin";

            var fileBytes = await iServiceRequestAutoMapper.GetServiceRequestReportByDateRangeAsync(fromDate, toDate, reportFormat);

            if (fileBytes == null || fileBytes.Length == 0)
            {
                return NotFound(new { success = false, message = "No data found for the selected date range." });
            }

            if (string.IsNullOrEmpty(reportTitle))
            {
                reportTitle = "Lead Report";
                fromDate = Convert.ToDateTime(fromDate.ToString("yyyy-MM-dd"));
                toDate = Convert.ToDateTime(toDate.ToString("yyyy-MM-dd"));
            }

            var fileExtension = reportFormat?.ToLower() == "csv" ? "csv" : "xlsx";
            var fileName = $"{reportTitle}_{fromDate:yyyy-MM-dd}_{toDate:yyyy-MM-dd}.{fileExtension}";

            var contentType = fileExtension == "csv"
                ? "text/csv"
                : "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

            return File(fileBytes, contentType, fileName);
        }

    }
}

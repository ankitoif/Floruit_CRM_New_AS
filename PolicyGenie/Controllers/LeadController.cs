using Microsoft.AspNetCore.Mvc;
using OIF.Cams.Business.AutoMapping.Lead;
using OIF.Cams.Model.LeadVM;

namespace PolicyGenie.Controllers
{
    public class LeadController : Controller
    {
        private readonly ILeadAutoMapper _ILeadAutoMapper;
        private readonly ILogger<LeadController> _ILogger;
        public LeadController(ILeadAutoMapper ILeadAutoMapper, ILogger<LeadController> ILogger)
        {
            _ILeadAutoMapper = ILeadAutoMapper;
            _ILogger = ILogger;
        }
        [HttpGet]
        public async Task<IActionResult> CreateLead()
        {
            //var model = await iServiceRequestAutoMapper.GetCreateServiceRequestModelAsync();
            LeadViewModel model = new LeadViewModel();
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> CreateLead(LeadViewModel model)
        {
            if (/*ModelState.IsValid && */model.TradeLicense != null)
            {
                string currentUser = HttpContext.User.Identity.Name;
                if (currentUser != null)
                {
                    model.CurrentUser = currentUser;
                }
                byte[] tradeLicenseBytes = null;
                byte[] vatCertificateBytes = null;

                if (model.TradeLicense != null && model.TradeLicense.Length > 0)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await model.TradeLicense.CopyToAsync(memoryStream);
                        tradeLicenseBytes = memoryStream.ToArray();
                    }
                }

                if (model.VATCertificate != null && model.VATCertificate.Length > 0)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await model.VATCertificate.CopyToAsync(memoryStream);
                        vatCertificateBytes = memoryStream.ToArray();
                    }
                }

                var result = await _ILeadAutoMapper.CreateLeadAsync(model);

                TempData["Success"] = "Lead Created Successfully!";
                return RedirectToAction("ServiceRequestDashboard","ServiceRequest", new { dashboardType = "TotalTask"});
            }
            TempData["Error"] = "Lead Creation Failed!";
            return RedirectToAction("ServiceRequestDashboard", "ServiceRequest", new { dashboardType = "TotalTask" });
        }

        public async Task<IActionResult> LeadDashboard()
        {
            var currentUser = User.Identity.Name;
            //var isAdmin = User.IsInRole("Admin");
            var activeRole = HttpContext.Session.GetString("ActiveRole");

            bool isAdmin = activeRole == "Admin";

            var model = await _ILeadAutoMapper.GetLeadDashboard(currentUser, isAdmin);
            if (model == null || !model.Any())
            {
                model = new List<LeadViewGridData>();
            }
            ViewBag.TotalTask = model.Count;
            ViewBag.CurrentUserFullName = model.FirstOrDefault()?.CurrentUserFullName ?? "Unknown User";
            return View("~/Views/ServiceRequest/ServiceRequestDashboard.cshtml", model);
        }

        [HttpGet]
        public async Task<IActionResult> GetLeadByTradeLicense(string tradeLicenseNumber)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(tradeLicenseNumber))
                    return Json(new { success = false, message = "Invalid input." });

                var lead = await _ILeadAutoMapper.GetLeadByTradeLicense(tradeLicenseNumber);

                if (lead == null)
                    return Json(new { success = false, message = "Lead not found." });

                return Json(new { success = true, lead });
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<IActionResult> GetLeadDetails(string productRefNo)
        {
            try
            {
                var activeRole = HttpContext.Session.GetString("ActiveRole");
                ViewBag.ActiveRole = activeRole;
                // Fetch the lead details from your database/service
                var lead = await _ILeadAutoMapper.GetLeadByProductRefNo(productRefNo);

                if (lead == null)
                {
                    return NotFound();
                }

                // Return a partial view with the lead details
                return PartialView("~/Views/Lead/_LeadDetailsPartial.cshtml", lead);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error getting lead details", ex);
                return PartialView("_ErrorPartial", "Error loading lead details");
            }
        }


        [HttpPost]
        public async Task<JsonResult> UpdateLeadDetails([FromBody] LeadDetailModel model)
        {
            try
            {
                var activeRole = HttpContext.Session.GetString("ActiveRole");
                model.activeUserRole = activeRole;
                string currentUser = HttpContext.User.Identity.Name;
                if (currentUser != null)
                {
                    model.currentUser = currentUser;
                }
                var selectedBank = model.Bank;
                var selectedStatus = model.Status;
                // Validate the model
                //if (!ModelState.IsValid)
                //{
                //    return Json(new { success = false, message = "Invalid data" });
                //}

                //Update the lead in your database/ service
                var result = await _ILeadAutoMapper.UpdateLeadDetails(model);

                if (result == true)
                {
                    return Json(new { success = true, message = "Saved Successfully" });
                }
                else
                {
                    return Json(new { success = false, message = "Failed to Save" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<JsonResult> GetAgencies()
        {
            var agencies = await _ILeadAutoMapper.GetAllAgencies();
            return Json(agencies);
        }

        [HttpGet]
        public async Task<JsonResult> GetAgentsByAgency(int agencyId)
        {
            var agents = await _ILeadAutoMapper.GetAgentsByAgency(agencyId);
            return Json(agents);
        }

    }
}

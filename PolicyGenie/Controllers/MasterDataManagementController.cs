using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using OIF.Cams.Business.AutoMapping.MasterDataManagement;
using OIF.Cams.CrossCutting;
using OIF.Cams.Data.DAC.AppDbContext;
using OIF.Cams.Data.Repository.MasterDataManagement;
using OIF.Cams.Model.ViewModels;
using System.Threading.Tasks;

namespace PolicyGenie.Controllers
{
    public class MasterDataManagementController : Controller
    {
        private readonly ILogger<MasterDataManagementController> logger;
        private readonly IMasterDataManagementAutoMapper iMasterDataManagementAutoMapper;
        public MasterDataManagementController(IMasterDataManagementAutoMapper _iMasterDataManagementAutoMapper, ILogger<MasterDataManagementController> _logger)
        {
            logger = _logger;
            iMasterDataManagementAutoMapper = _iMasterDataManagementAutoMapper;
        }
        public async Task<IActionResult> ManageBranch()
        {
            try
            {
                var org = await iMasterDataManagementAutoMapper.GetOrganisationList();
                var branchList = await iMasterDataManagementAutoMapper.GetAllBranchList();
                BranchViewModel model = new BranchViewModel
                {
                    CustomerOrgs = org,
                    BranchList = branchList 
                };
                return View(model);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, CrossCutting_Constants.ControllerPolicy);
                return View("Error");
            }           
        }
        [HttpPost]
        public async Task<IActionResult> ManageBranch(BranchViewModel branchData)
        {
            try
            {
                var org = await iMasterDataManagementAutoMapper.GetOrganisationList();
                var branchList = await iMasterDataManagementAutoMapper.GetAllBranchList();
                if (!ModelState.IsValid)
                {                  
                    BranchViewModel model = new BranchViewModel
                    {
                        CustomerOrgs = org,
                        BranchList = branchList
                    };
                    return View(model);
                }
                var res = await iMasterDataManagementAutoMapper.SaveBranchData(branchData);
                if (res)
                {
                    TempData["Success"] = "Branch created successfully!";
                    return RedirectToAction(nameof(ManageBranch));
                }
                TempData["Error"] = "Failed to create branch.";
                branchData.CustomerOrgs = await iMasterDataManagementAutoMapper.GetOrganisationList();
                branchData.BranchList = await iMasterDataManagementAutoMapper.GetAllBranchList();
                return View(branchData);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, CrossCutting_Constants.ControllerPolicy);
                return View("Error");
            }           
        }
        public async Task<IActionResult> ManageServiceType()
        {
            try
            {
                var org = await iMasterDataManagementAutoMapper.GetOrganisationList();
                var serviceTypeList = await iMasterDataManagementAutoMapper.GetAllServiceTypeList();
                ServiceTypeViewModel model = new ServiceTypeViewModel();
                model.CustomerOrgs = org;
                model.ServiceTypeList = serviceTypeList;
                return View(model);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, CrossCutting_Constants.ControllerPolicy);
                return View("Error");
            }           
        }
        [HttpPost]
        public async Task<IActionResult> ManageServiceType(ServiceTypeViewModel serviceTypeData)
        {
            try
            {
                var org = await iMasterDataManagementAutoMapper.GetOrganisationList();
                var serviceTypeList = await iMasterDataManagementAutoMapper.GetAllServiceTypeList();
                if (!ModelState.IsValid)
                {                    
                    ServiceTypeViewModel model = new ServiceTypeViewModel
                    {
                        CustomerOrgs = org,
                        ServiceTypeList = serviceTypeList
                    };
                    return View(model);
                }
                var res = await iMasterDataManagementAutoMapper.SaveServiceTypeData(serviceTypeData);
                if (res)
                {
                    TempData["Success"] = "ServiceType created successfully!";
                    return RedirectToAction(nameof(ManageServiceType));
                }
                TempData["Error"] = "Failed to create serviceType!";
                serviceTypeData.CustomerOrgs = await iMasterDataManagementAutoMapper.GetOrganisationList();
                serviceTypeData.ServiceTypeList = await iMasterDataManagementAutoMapper.GetAllServiceTypeList();
                return View(serviceTypeData);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, CrossCutting_Constants.ControllerPolicy);
                return View("Error");
            }            
        }
        [HttpPost]
        public async Task<IActionResult> UpdateBranchStatus(long branchId, bool isValid)
        {
            try
            {
                var result = await iMasterDataManagementAutoMapper.UpdateBranchStatus(branchId, isValid);
                if (result)
                {
                    return Json(new { success = true, message = "Branch status updated successfully!" });
                }
                else
                {
                    return Json(new { success = false, message = "Failed to update branch status!" });
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, CrossCutting_Constants.ControllerPolicy);
                return StatusCode(500, "Internal server error");
            }
        }
        [HttpPost]
        public async Task<IActionResult> UpdateServiceStatus(long serviceTypeId, bool isValid)
        {
            try
            {
                var result = await iMasterDataManagementAutoMapper.UpdateServiceStatus(serviceTypeId, isValid);
                if (result)
                {
                    return Json(new { success = true, message = "Service Type status updated successfully!" });
                }
                else
                {
                    return Json(new { success = false, message = "Failed to update Service Type status!" });
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, CrossCutting_Constants.ControllerPolicy);
                return StatusCode(500, "Internal server error");
            }
        }

    }
}

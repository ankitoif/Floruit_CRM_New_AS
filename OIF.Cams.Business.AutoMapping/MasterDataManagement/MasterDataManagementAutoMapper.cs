using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using OIF.Cams.Business.AutoMapping.ServiceRequest;
using OIF.Cams.CrossCutting;
using OIF.Cams.Data.Repository.MasterDataManagement;
using OIF.Cams.Data.Repository.ServiceRequest;
using OIF.Cams.Model.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OIF.Cams.Business.AutoMapping.MasterDataManagement
{
    public class MasterDataManagementAutoMapper : IMasterDataManagementAutoMapper
    {
        private readonly ILogger<MasterDataManagementAutoMapper> logger;
        private readonly IMasterDataManagementRepo iMasterDataManagementRepo;
        public MasterDataManagementAutoMapper(ILogger<MasterDataManagementAutoMapper> _logger, IMasterDataManagementRepo _iMasterDataManagementRepo)
        {
            logger = _logger;
            iMasterDataManagementRepo = _iMasterDataManagementRepo;
        }
        public async Task<List<SelectListItem>> GetOrganisationList()
        {
			try
			{
                return await iMasterDataManagementRepo.GetOrganisationList();
			}
			catch (Exception ex)
			{
                logger.LogError(ex, CrossCutting_Constants.AMPolicy);
                return new List<SelectListItem>();
			}
        }
        public async Task<bool> SaveBranchData(BranchViewModel branchData)
        {
            try
            {
                return await iMasterDataManagementRepo.SaveBranchData(branchData);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, CrossCutting_Constants.AMPolicy);
                return false;
            }
        }
        public async Task<bool> SaveServiceTypeData(ServiceTypeViewModel serviceTypeData)
        {
            try
            {
                return await iMasterDataManagementRepo.SaveServiceTypeData(serviceTypeData);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, CrossCutting_Constants.AMPolicy);
                return false;
            }
        }
        public async Task<List<BranchListViewModel>> GetAllBranchList()
        {
            try
            {
                return await iMasterDataManagementRepo.GetAllBranchList();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, CrossCutting_Constants.AMPolicy);
                return new List<BranchListViewModel>();
            }
        }
        public async Task<List<ServiceTypeListViewModel>> GetAllServiceTypeList()
        {
            try
            {
                return await iMasterDataManagementRepo.GetAllServiceTypeList();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, CrossCutting_Constants.AMPolicy);
                return new List<ServiceTypeListViewModel>();
            }
        }
        public async Task<bool> UpdateBranchStatus(long branchId, bool isValid)
        {
            try
            {
                return await iMasterDataManagementRepo.UpdateBranchStatus(branchId, isValid);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, CrossCutting_Constants.AMPolicy);
                return false;
            }
        }
        public async Task<bool> UpdateServiceStatus(long serviceTypeId, bool isValid)
        {
            try
            {
                return await iMasterDataManagementRepo.UpdateServiceStatus(serviceTypeId, isValid);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, CrossCutting_Constants.AMPolicy);
                return false;
            }
        }
    }
}

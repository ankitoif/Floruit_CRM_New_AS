using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OIF.Cams.CrossCutting;
using OIF.Cams.Data.DAC;
using OIF.Cams.Data.DAC.AppDbContext;
using OIF.Cams.Data.Repository.ServiceRequest;
using OIF.Cams.Model.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OIF.Cams.Data.Repository.MasterDataManagement
{
    public class MasterDataManagementRepo : IMasterDataManagementRepo
    {
        private readonly ILogger<MasterDataManagementRepo> logger;
        private readonly CamsDbContext Context;
        public MasterDataManagementRepo(ILogger<MasterDataManagementRepo> _logger, CamsDbContext _Context)
        {
            logger = _logger;
            Context = _Context;
        }
        public async Task<List<SelectListItem>> GetOrganisationList()
        {
            try
            {
                return await Context.TblMstCustomerOrgs.AsNoTracking().Select(x => new SelectListItem
                {
                    Value = x.CustomerOrgId.ToString(),
                    Text = x.CustomerOrgName
                }).ToListAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, CrossCutting_Constants.RepositoryPolicy);
                return new List<SelectListItem>();
            }
        }
        public async Task<bool> SaveBranchData(BranchViewModel branchData)
        {
            try
            {
                var branch = new TblMstBranch
                {
                    CustomerOrgId = branchData.CustomerOrgId,
                    BranchName = branchData.BranchName,
                    BranchCode = branchData.BranchCode,
                    BranchAddress = branchData.BranchAddress,
                    IsValid = true,
                    CreatedDateTime = DateTime.UtcNow,
                    ModifiedDateTime = DateTime.UtcNow
                };
                Context.TblMstBranches.Add(branch);
                await Context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, CrossCutting_Constants.RepositoryPolicy);
                return false;
            }
        }
        public async Task<bool> SaveServiceTypeData(ServiceTypeViewModel serviceTypeData)
        {
            try
            {
                var service = new TblMstServiceType
                {
                    CustomerOrgId = serviceTypeData.CustomerOrgId,
                    ServiceTypeDescription = serviceTypeData.ServiceTypeDescription,
                    IsValid = true,
                    CreatedDateTime = DateTime.UtcNow,
                    ModifiedDateTime = DateTime.UtcNow
                };
                Context.TblMstServiceTypes.Add(service);
                await Context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, CrossCutting_Constants.RepositoryPolicy);
                return false;
            }
        }
        public async Task<List<BranchListViewModel>> GetAllBranchList()
        {
            try
            {
                return await Context.TblMstBranches
     .AsNoTracking()
     .Include(b => b.CustomerOrg)
     .OrderByDescending(b => b.CreatedDateTime)
     .Select(x => new BranchListViewModel
     {
         BranchId = x.BranchId,
         BranchName = x.BranchName,
         BranchCode = x.BranchCode,
         BranchAddress = x.BranchAddress,
         Organisation = x.CustomerOrg != null ? x.CustomerOrg.CustomerOrgName : "N/A",
         CreatedDateTime = x.CreatedDateTime,
         ModifiedDateTime = x.ModifiedDateTime,
         IsValid = x.IsValid
     }).ToListAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, CrossCutting_Constants.RepositoryPolicy);
                return new List<BranchListViewModel>();
            }
        }
        public async Task<List<ServiceTypeListViewModel>> GetAllServiceTypeList()
        {
            try
            {
                return await Context.TblMstServiceTypes
     .AsNoTracking()
     .Include(b => b.CustomerOrg)
     .OrderByDescending(b => b.CreatedDateTime)
     .Select(x => new ServiceTypeListViewModel
     {
         ServiceTypeId = x.ServiceTypeId,
         ServiceTypeDescription = x.ServiceTypeDescription,
         Organisation = x.CustomerOrg != null ? x.CustomerOrg.CustomerOrgName : "N/A",
         CreatedDateTime = x.CreatedDateTime,
         ModifiedDateTime = x.ModifiedDateTime,
         IsValid = x.IsValid
     }).ToListAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, CrossCutting_Constants.RepositoryPolicy);
                return new List<ServiceTypeListViewModel>();
            }
        }
        public async Task<bool> UpdateBranchStatus(long branchId, bool isValid)
        {
            try
            {
                var branch = await Context.TblMstBranches.FindAsync(branchId);
                if (branch != null)
                {
                    branch.IsValid = isValid;
                    branch.ModifiedDateTime = DateTime.UtcNow;
                    await Context.SaveChangesAsync();
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, CrossCutting_Constants.RepositoryPolicy);
                return false;
            }            
        }
        public async Task<bool> UpdateServiceStatus(long serviceTypeId, bool isValid)
        {
            try
            {
                var serviceType = await Context.TblMstServiceTypes.FindAsync(serviceTypeId);
                if (serviceType != null)
                {
                    serviceType.IsValid = isValid;
                    serviceType.ModifiedDateTime = DateTime.UtcNow;
                    await Context.SaveChangesAsync();
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, CrossCutting_Constants.RepositoryPolicy);
                return false;
            }
        }
    }
}

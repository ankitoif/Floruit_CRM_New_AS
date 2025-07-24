using Microsoft.AspNetCore.Mvc.Rendering;
using OIF.Cams.Data.DAC.AppDbContext;
using OIF.Cams.Model.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OIF.Cams.Data.Repository.MasterDataManagement
{
    public interface IMasterDataManagementRepo
    {
        Task<List<SelectListItem>> GetOrganisationList();
        Task<bool> SaveBranchData(BranchViewModel branchData);
        Task<bool> SaveServiceTypeData(ServiceTypeViewModel serviceTypeData);
        Task<List<BranchListViewModel>> GetAllBranchList();
        Task<List<ServiceTypeListViewModel>> GetAllServiceTypeList();
        Task<bool> UpdateBranchStatus(long branchId, bool isValid);
        Task<bool> UpdateServiceStatus(long serviceTypeId, bool isValid);
    }
}

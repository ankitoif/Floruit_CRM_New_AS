using OIF.Cams.Model.LeadVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OIF.Cams.Data.Repository.Lead
{
    public interface ILeadRepo
    {
        Task<bool> CreateLeadAsync(LeadViewModel model);
        Task<List<LeadViewGridData>> GetLeadDashboard(string currentUser, bool isAdmin);
        Task<LeadViewModel> GetLeadByTradeLicense(string tradeLicenseNumber);
        Task<LeadDetailModel> GetLeadByProductRefNo(string productRefNo);
        Task<bool> UpdateLeadDetails(LeadDetailModel model);
        Task<List<AgencyModel>> GetAllAgencies();
        Task<List<AgentModel>> GetAgentsByAgency(int agencyId);
    }
}

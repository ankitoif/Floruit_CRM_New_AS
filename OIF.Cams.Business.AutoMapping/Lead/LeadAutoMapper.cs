using Microsoft.Extensions.Logging;
using OIF.Cams.CrossCutting;
using OIF.Cams.Data.Repository.Lead;
using OIF.Cams.Model.LeadVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OIF.Cams.Business.AutoMapping.Lead
{
    public class LeadAutoMapper : ILeadAutoMapper
    {
        private readonly ILeadRepo _ILeadRepo;
        private readonly ILogger<ILeadAutoMapper> _ILogger;
        public LeadAutoMapper(ILeadRepo ILeadRepo, ILogger<ILeadAutoMapper> ILogger)
        {
            _ILeadRepo = ILeadRepo;
            _ILogger = ILogger;
        }
        public async Task<bool> CreateLeadAsync(LeadViewModel model)
        {
            try
            {
                return await _ILeadRepo.CreateLeadAsync(model);
            }
            catch (Exception ex)
            {
                _ILogger.LogError(ex, CrossCutting_Constants.AMPolicy);
                return false;
            }
        }

        public async Task<List<LeadViewGridData>> GetLeadDashboard(string currentUser, bool isAdmin)
        {
            try
            {
                return await _ILeadRepo.GetLeadDashboard(currentUser, isAdmin);
            }
            catch (Exception ex)
            {
                _ILogger.LogError(ex, CrossCutting_Constants.AMPolicy);
                return new List<LeadViewGridData>();
            }
        }
        public async Task<LeadViewModel> GetLeadByTradeLicense(string tradeLicenseNumber)
        {
            return await _ILeadRepo.GetLeadByTradeLicense(tradeLicenseNumber);
        }
        public async Task<LeadDetailModel> GetLeadByProductRefNo(string productRefNo)
        {
            return await _ILeadRepo.GetLeadByProductRefNo(productRefNo);
        }

        public async Task<bool> UpdateLeadDetails(LeadDetailModel model)
        {
            return await _ILeadRepo.UpdateLeadDetails(model);
        }

        public async Task<List<AgencyModel>> GetAllAgencies()
        {
            try
            {
                return await _ILeadRepo.GetAllAgencies();
            }
            catch (Exception ex)
            {
                _ILogger.LogError(ex, CrossCutting_Constants.AMPolicy);
                return new List<AgencyModel>();
            }            
        }

        public async Task<List<AgentModel>> GetAgentsByAgency(int agencyId)
        {
            try
            {
                return await _ILeadRepo.GetAgentsByAgency(agencyId);
            }
            catch (Exception ex)
            {
                _ILogger.LogError(ex, CrossCutting_Constants.AMPolicy);
                return new List<AgentModel>();
            }
        }

    }
}

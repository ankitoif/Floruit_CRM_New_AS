using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OIF.Cams.CrossCutting;
using OIF.Cams.Data.DAC;
using OIF.Cams.Data.DAC.AppDbContext;
using OIF.Cams.Data.Repository.Account;
using OIF.Cams.Model.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OIF.Cams.Data.Repository.ServiceRequest
{
    public class ServiceRequestRepo : IServiceRequestRepo
    {
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly ILogger<ServiceRequestRepo> logger;
        private readonly CamsDbContext Context;
        public ServiceRequestRepo(ILogger<ServiceRequestRepo> _logger, CamsDbContext _context, SignInManager<ApplicationUser> _signInManager, UserManager<ApplicationUser> _userManager, RoleManager<IdentityRole> _roleManager)
        {
            logger = _logger;
            Context = _context;
            signInManager = _signInManager;
            userManager = _userManager;
            roleManager = _roleManager;
        }
        public async Task<ServiceRequestViewModel> GetCreateServiceRequestModelAsync()
        {
            try
            {
                var serviceTypeDocs = await Context.TblMstServiceTypeDocuments
                    .AsNoTracking()
                    .GroupBy(d => d.ServiceTypeId.Value)
                    .ToDictionaryAsync(
                        g => g.Key,
                        g => g.Select(x => new ServiceTypeDocumentDto
                        {
                            ServiceTypeDocumentId = x.ServiceTypeDocumentId,
                            DocumentCategory = x.DocumentCategory
                        }).ToList());

                return new ServiceRequestViewModel
                {
                    CrmList = await Context.TblMstCrms
                            .Select(c => new SelectListItem { Value = c.Crmid.ToString(), Text = c.Crmname })
                            .ToListAsync(),
                    ServiceTypeList = await Context.TblMstServiceTypes
                            .Select(s => new SelectListItem { Value = s.ServiceTypeId.ToString(), Text = s.ServiceTypeDescription })
                            .ToListAsync(),

                            ServiceTypeDocuments = serviceTypeDocs
                };
            }
            catch (Exception ex)
            {
                logger.LogError(ex, CrossCutting_Constants.RepositoryPolicy);
                return new ServiceRequestViewModel
                {
                    CrmList = new List<SelectListItem>(),
                    ServiceTypeList = new List<SelectListItem>()
                };
            }            
        }

        public async Task<bool> SaveServiceRequestAsync(ServiceRequestViewModel model, List<ServiceRequestDocument> documents, string uploadPath, string currentUser)
        {
            try
            {
                var orgId =  Context.TblMstCrms.AsNoTracking().Where(x => x.Crmid == model.CrmId && x.IsValid == true).Select(x => x.CustomerOrgId).FirstOrDefault();
                var serviceRequest = new TblServiceRequest
                {
                    PolicyNo = model.PolicyNumber,
                    PolicyHolderName = model.PolicyHolderName,
                    Crmid = model.CrmId,
                    CustomerOrgId = orgId != null ? orgId : null,
                    ServiceTypeId = model.ServiceTypeId,
                    CreatedDateTime = DateTime.Now,
                    ModifiedDateTime = DateTime.Now,
                    IsValid = true,
                    IsPolicyVerified = true,
                    StatusId = 1,
                    ServiceReqCreatedBy = currentUser
                
                };

                Context.TblServiceRequests.Add(serviceRequest);
                await Context.SaveChangesAsync();

                if (documents != null && documents.Count > 0)
                {
                    if (!Directory.Exists(uploadPath))
                        Directory.CreateDirectory(uploadPath);

                    foreach (var docItem in documents)
                    {
                        var file = docItem.File;
                        if (file == null || file.Length == 0) continue;

                        var uniqueFileName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
                        var fullPath = Path.Combine(uploadPath, uniqueFileName);

                        await using (var stream = new FileStream(fullPath, FileMode.Create))
                        {
                            await file.CopyToAsync(stream);
                        }

                        Context.TblServiceReqDocumentUploads.Add(new TblServiceReqDocumentUpload
                        {
                            ServiceTypeDocumentId = docItem.ServiceTypeDocumentId,
                            ServiceRequestId = serviceRequest.ServiceRequestId,
                            DocumentUploadFilePath = Path.Combine("uploads", uniqueFileName),
                            CreatedDateTime = DateTime.Now,
                            UploadedDateTime = DateTime.Now,
                            IsValid = true
                        });
                    }

                    await Context.SaveChangesAsync();
                }

                return true;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, CrossCutting_Constants.RepositoryPolicy);
                return false;
            }
        }

        public async Task<ServiceRequestDashboardViewModel> GetServiceRequestDashboardAsync(string status, string currentUser, bool isAdmin)
        {
            var model = new ServiceRequestDashboardViewModel();
            var connection = Context.Database.GetDbConnection();

            try
            {
                using (var cmd = connection.CreateCommand())
                {
                    cmd.CommandText = "usp_GetServiceDashboardData";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@Status", status));
                    cmd.Parameters.Add(new SqlParameter("@CurrentUser", currentUser));
                    cmd.Parameters.Add(new SqlParameter("@IsAdmin", isAdmin));

                    await connection.OpenAsync();

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (isAdmin == false)
                        {
                            if (await reader.ReadAsync())
                            {
                                model.TotalCount = reader.GetInt32(0);
                            }

                            if (await reader.NextResultAsync())
                            {
                                var leads = new List<ServiceRequestDashboardModel>();
                                while (await reader.ReadAsync())
                                {
                                    leads.Add(new ServiceRequestDashboardModel
                                    {
                                        LeadId = Convert.ToInt64(reader["LeadID"]),
                                        ProductRefNo = reader["ProductRefNo"].ToString(),
                                        CompanyName = reader["CompanyName"].ToString(),
                                        LeadCreatedDateTime = Convert.ToDateTime(reader["LeadCreatedDateTime"]),
                                        LeadType = reader["LeadType"].ToString(),
                                        CSP_SalesName = reader["CSP_SalesName"].ToString(),
                                        LeadSource = reader["LeadSource"].ToString(),
                                        LeadStatus = reader["Description"].ToString(),
                                        CreatedBy = reader["CreatedBy"].ToString(),
                                        Address = reader["Address"].ToString(),
                                    });
                                }
                                model.Leads = leads;
                            }
                        }
                        else 
                        {
                            var statusCounts = new List<StatusCount>();
                            while (await reader.ReadAsync())
                            {
                                statusCounts.Add(new StatusCount
                                {
                                    Status = reader["Description"].ToString(),
                                    Count = Convert.ToInt32(reader["Count"])
                                });
                            }
                            model.StatusCounts = statusCounts;

                            if (await reader.NextResultAsync())
                            {
                                var leads = new List<ServiceRequestDashboardModel>();
                                while (await reader.ReadAsync())
                                {
                                    leads.Add(new ServiceRequestDashboardModel
                                    {
                                        LeadId = Convert.ToInt64(reader["LeadID"]),
                                        ProductRefNo = reader["ProductRefNo"].ToString(),
                                        CompanyName = reader["CompanyName"].ToString(),
                                        LeadCreatedDateTime = Convert.ToDateTime(reader["LeadCreatedDateTime"]),
                                        LeadType = reader["LeadType"].ToString(),
                                        CSP_SalesName = reader["CSP_SalesName"].ToString(),
                                        LeadSource = reader["LeadSource"].ToString(),
                                        LeadStatus = reader["Description"].ToString(),
                                        CreatedBy = reader["CreatedBy"].ToString(),
                                        Address = reader["Address"].ToString(),
                                    });
                                }
                                model.Leads = leads;
                            }
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred while fetching service request dashboard data.");
                return new ServiceRequestDashboardViewModel();
            }
            return model;
        }
        public async Task<List<ServiceRequestDownloadReportModel>> GetServiceRequestReportData(string dashboardType, string currentUser, bool isAdmin)
        {
            var result = new List<ServiceRequestDownloadReportModel>();
            try
            {
                var connection = Context.Database.GetDbConnection();

                if (connection.State != ConnectionState.Open)
                    await connection.OpenAsync();

                using (var cmd = connection.CreateCommand())
                {
                    cmd.CommandText = "usp_GetServiceDashboardData";
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add(new SqlParameter("@Status", dashboardType));
                    cmd.Parameters.Add(new SqlParameter("@CurrentUser", string.IsNullOrEmpty(currentUser) ? DBNull.Value : (object)currentUser));
                    cmd.Parameters.Add(new SqlParameter("@IsAdmin", isAdmin));

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        await reader.NextResultAsync();

                        while (await reader.ReadAsync())
                        {
                            var item = new ServiceRequestDownloadReportModel
                            {
                                ProductRefNo = reader["ProductRefNo"]?.ToString(),
                                CompanyName = reader["CompanyName"]?.ToString(),
                                LeadCreatedDateTime = reader.GetDateTime(reader.GetOrdinal("LeadCreatedDateTime")),
                                LeadType = reader["LeadType"]?.ToString(),
                                CSP_SalesName = reader["CSP_SalesName"]?.ToString(),
                                LeadSource = reader["LeadSource"]?.ToString(),
                                Address = reader["Address"]?.ToString(),
                                CreatedBy = reader["CreatedBy"]?.ToString(),
                                LeadStatus = reader["Description"]?.ToString()
                            };

                            result.Add(item);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred while fetching service request dashboard data.");
            }

            return result;
        }
        public async Task<List<ServiceRequestDownloadReportModel>> GetServiceRequestReportByDateRangeAsync(DateTime fromDate, DateTime toDate)
        {
            var result = new List<ServiceRequestDownloadReportModel>();
            try
            {
                var connection = Context.Database.GetDbConnection();

                if (connection.State != ConnectionState.Open)
                    await connection.OpenAsync();

                using (var cmd = connection.CreateCommand())
                {
                    cmd.CommandText = "usp_GetServiceReportByDate";
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add(new SqlParameter("@FromDate", fromDate));
                    cmd.Parameters.Add(new SqlParameter("@ToDate", toDate));

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var item = new ServiceRequestDownloadReportModel
                            {
                                ProductRefNo = reader["ProductRefNo"]?.ToString(),
                                CompanyName = reader["CompanyName"]?.ToString(),
                                LeadCreatedDateTime = reader.GetDateTime(reader.GetOrdinal("LeadCreatedDateTime")),
                                LeadType = reader["LeadType"]?.ToString(),
                                CSP_SalesName = reader["CSP_SalesName"]?.ToString(),
                                LeadSource = reader["LeadSource"]?.ToString(),
                                Address = reader["Address"]?.ToString(),
                                CreatedBy = reader["CreatedBy"]?.ToString(),
                                LeadStatus = reader["Description"]?.ToString()
                            };

                            result.Add(item);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred while fetching service request dashboard data.");
            }

            return result;
        }

    }
}

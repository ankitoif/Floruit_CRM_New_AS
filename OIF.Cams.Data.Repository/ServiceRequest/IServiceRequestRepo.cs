using Microsoft.AspNetCore.Http;
using OIF.Cams.Model.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;


namespace OIF.Cams.Data.Repository.ServiceRequest
{
    public interface IServiceRequestRepo
    {
        Task<ServiceRequestViewModel> GetCreateServiceRequestModelAsync();
        Task<bool> SaveServiceRequestAsync(ServiceRequestViewModel model, List<ServiceRequestDocument> documents, string uploadPath, string currentUser);
        Task<ServiceRequestDashboardViewModel> GetServiceRequestDashboardAsync(string status, string currentUser, bool isAdmin);
        Task<List<ServiceRequestDownloadReportModel>> GetServiceRequestReportData(string dashboardType, string currentUser, bool isAdmin);
        Task<List<ServiceRequestDownloadReportModel>> GetServiceRequestReportByDateRangeAsync(DateTime fromDate, DateTime toDate);

    }
}

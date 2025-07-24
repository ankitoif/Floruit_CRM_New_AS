using Microsoft.AspNetCore.Http;
using OIF.Cams.Model.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OIF.Cams.Business.AutoMapping.ServiceRequest
{
    public interface IServiceRequestAutoMapper
    {
        Task<ServiceRequestViewModel> GetCreateServiceRequestModelAsync();
        Task<bool> SaveServiceRequestAsync(ServiceRequestViewModel model, List<ServiceRequestDocument> documents, string uploadPath, string currentUser);
        Task<ServiceRequestDashboardViewModel> GetServiceRequestDashboardAsync(string dashboardType, string currentUser, bool isAdmin);
        Task<byte[]> GetServiceRequestReportExcelAsync(string dashboardType, string currentUser, bool isAdmin);
        Task<byte[]> GetServiceRequestReportByDateRangeAsync(DateTime fromDate, DateTime toDate, string reportFormat);
    }
}

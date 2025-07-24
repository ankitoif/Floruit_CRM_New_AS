using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using OfficeOpenXml;
using OIF.Cams.CrossCutting;
using OIF.Cams.Data.Repository.ServiceRequest;
using OIF.Cams.Model.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace OIF.Cams.Business.AutoMapping.ServiceRequest
{
    public class ServiceRequestAutoMapper : IServiceRequestAutoMapper
    {
        private readonly ILogger<ServiceRequestAutoMapper> logger;
        private readonly IServiceRequestRepo serviceRequestRepo;
        public ServiceRequestAutoMapper(ILogger<ServiceRequestAutoMapper> _logger, IServiceRequestRepo _serviceRequestRepo)
        {
            logger = _logger;
            serviceRequestRepo = _serviceRequestRepo;
        }

        public async Task<ServiceRequestViewModel> GetCreateServiceRequestModelAsync()
        {
            try
            {
                return await serviceRequestRepo.GetCreateServiceRequestModelAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, CrossCutting_Constants.AMPolicy);
                return new ServiceRequestViewModel();
            }
        }

        public async Task<ServiceRequestDashboardViewModel> GetServiceRequestDashboardAsync(string dashboardType, string currentUser, bool isAdmin)
        {
            try
            {
                return await serviceRequestRepo.GetServiceRequestDashboardAsync(dashboardType, currentUser, isAdmin);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, CrossCutting_Constants.AMPolicy);
                return new ServiceRequestDashboardViewModel();
            }
        }

        public async Task<bool> SaveServiceRequestAsync(ServiceRequestViewModel model, List<ServiceRequestDocument> documents, string uploadPath, string currentUser)
        {
            try
            {
                return await serviceRequestRepo.SaveServiceRequestAsync(model, documents, uploadPath, currentUser);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, CrossCutting_Constants.AMPolicy);
                return false;
            }
        }
        public async Task<byte[]> GetServiceRequestReportExcelAsync(string dashboardType, string currentUser, bool isAdmin)
        {
            try
            {
                var reportData = await serviceRequestRepo.GetServiceRequestReportData(dashboardType, currentUser, isAdmin);

                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                using var package = new ExcelPackage();
                var worksheet = package.Workbook.Worksheets.Add("Lead Report");

                // Add header with yellow background
                worksheet.Cells["A1"].Value = "ProductRefNo";
                worksheet.Cells["B1"].Value = "CompanyName";
                worksheet.Cells["C1"].Value = "LeadSource";
                worksheet.Cells["D1"].Value = "LeadType";
                worksheet.Cells["E1"].Value = "Status";
                worksheet.Cells["F1"].Value = "Address";
                worksheet.Cells["G1"].Value = "CreatedBy";
                worksheet.Cells["H1"].Value = "LeadCreatedDateTime";

                using (var range = worksheet.Cells["A1:G1"])
                {
                    range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Yellow);
                    range.Style.Font.Bold = true;
                }

                // Add data rows
                for (int i = 0; i < reportData.Count; i++)
                {
                    var row = i + 2;
                    var item = reportData[i];
                    worksheet.Cells[row, 1].Value = item.ProductRefNo;
                    worksheet.Cells[row, 2].Value = item.CompanyName;
                    worksheet.Cells[row, 3].Value = item.LeadSource;
                    worksheet.Cells[row, 4].Value = item.LeadType;
                    worksheet.Cells[row, 5].Value = item.LeadStatus;
                    worksheet.Cells[row, 6].Value = item.Address;
                    worksheet.Cells[row, 7].Value = item.CreatedBy;
                    worksheet.Cells[row, 8].Value = item.LeadCreatedDateTime;
                }

                return package.GetAsByteArray();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, CrossCutting_Constants.AMPolicy);
                return Array.Empty<byte>();
            }
        }

        public async Task<byte[]> GetServiceRequestReportByDateRangeAsync(DateTime fromDate, DateTime toDate, string reportFormat)
        {
            try
            {
                var reportData = await serviceRequestRepo.GetServiceRequestReportByDateRangeAsync(fromDate, toDate);

                if (reportData != null && reportData.Count > 0)
                {
                    if (reportFormat == "csv")
                    {
                        return GenerateCsv(reportData);
                    }
                    else
                    {
                        return GenerateExcel(reportData);
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error while generating report.");
                return null;
            }
        }
        private byte[] GenerateCsv(List<ServiceRequestDownloadReportModel> data)
        {
            var sb = new StringBuilder();
            sb.AppendLine("ProductRefNo,CompanyName,LeadSource,LeadType,LeadStatus,CreatedBy,LeadCreatedDateTime,Address");

            foreach (var item in data)
            {
                sb.AppendLine(string.Join(',',
                    EscapeCsv(item.ProductRefNo.ToString()),
                    EscapeCsv(item.CompanyName.ToString()),
                    EscapeCsv(item.LeadSource.ToString()),
                    EscapeCsv(item.LeadType.ToString()),
                    EscapeCsv(item.LeadStatus.ToString()),
                    EscapeCsv(item.CreatedBy.ToString()),
                    EscapeCsv(item.LeadCreatedDateTime.ToString()),
                    EscapeCsv(item.Address.ToString())
                    ));
            }
            return Encoding.UTF8.GetBytes(sb.ToString());
        }

        private static string EscapeCsv(string value)
        {
            if (string.IsNullOrEmpty(value)) return "-";
            return value.Contains(',') || value.Contains('"') || value.Contains('\n')
                ? '"' + value.Replace("\"", "\"\"") + '"'
                : value;
        }

        private byte[] GenerateExcel(List<ServiceRequestDownloadReportModel> data)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("Service Report");

            var headers = new[] { "ProductRefNo", "CompanyName", "LeadSource", "LeadType", "LeadStatus", "CreatedBy", "LeadCreatedDateTime", "Address" };
            for (int col = 0; col < headers.Length; col++)
            {
                worksheet.Cells[1, col + 1].Value = headers[col];
            }
            using (var range = worksheet.Cells[1, 1, 1, headers.Length])
            {
                range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Yellow);
                range.Style.Font.Bold = true;
            }

            // Data rows
            for (int i = 0; i < data.Count; i++)
            {
                var row = i + 2;
                var item = data[i];
                worksheet.Cells[row, 1].Value = item.ProductRefNo;
                worksheet.Cells[row, 2].Value = item.CompanyName;
                worksheet.Cells[row, 3].Value = item.LeadSource;
                worksheet.Cells[row, 4].Value = item.LeadType;
                worksheet.Cells[row, 5].Value = item.LeadStatus;
                worksheet.Cells[row, 6].Value = item.CreatedBy;
                worksheet.Cells[row, 7].Value = item.LeadCreatedDateTime;
                worksheet.Cells[row, 8].Value = item.Address;
            }
            return package.GetAsByteArray();
        }

    }
}


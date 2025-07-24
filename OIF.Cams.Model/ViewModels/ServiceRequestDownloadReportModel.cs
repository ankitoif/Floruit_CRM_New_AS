using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OIF.Cams.Model.ViewModels
{
    public class ServiceRequestDownloadReportModel
    {
        public string ProductRefNo { get; set; }
        public string CompanyName { get; set; }
        public DateTime LeadCreatedDateTime { get; set; }
        public string LeadType { get; set; }
        public string CSP_SalesName { get; set; }
        public string LeadSource { get; set; }
        public string Address { get; set; }
        public string CreatedBy { get; set; }
        public string LeadStatus { get; set; }
    }
}

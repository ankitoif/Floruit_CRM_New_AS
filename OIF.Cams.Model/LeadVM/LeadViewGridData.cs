using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OIF.Cams.Model.LeadVM
{
    public class LeadViewGridData
    {
        public int LeadID { get; set; }
        public string LeadType { get; set; }
        public string ProductRefNo { get; set; }
        public int CustomerID { get; set; }
        public string CompanyName { get; set; }
        public string TradeLicenseNo { get; set; }
        public string Address { get; set; }
        public DateOnly? TradeLicenseExpDate { get; set; }
        public string VatTRNno { get; set; }
        public string LandLineNo { get; set; }
        public string LeadSource { get; set; }
        public string AgencyOrReferral { get; set; }
        public string CSP_SalesName { get; set; }
        public int BankID { get; set; }
        public DateOnly? IntroductionDate { get; set; }
        public int StatusID { get; set; }
        public DateOnly? AccountOpenedOrLoanDisbursedDate { get; set; }
        public bool Paid { get; set; }
        public decimal Amount { get; set; }
        public string InvoiceNo { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public DateTime? ModifiedDateTime { get; set; } 
        public bool IsValid { get; set; }
        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }
        public string? CurrentUserFullName { get; set; }
    }
}

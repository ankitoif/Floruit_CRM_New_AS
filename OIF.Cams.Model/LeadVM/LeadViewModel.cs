using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OIF.Cams.Model.LeadVM
{
    public class LeadViewModel
    {
        public string Product { get; set; }
        public IFormFile TradeLicense { get; set; }
        public IFormFile VATCertificate { get; set; }
        public string CompanyName { get; set; }
        public string TradeLicenseNumber { get; set; }
        public DateOnly TradeLicenseExpiry { get; set; } = new DateOnly();
        public string VAT { get; set; }
        public string vatTrn { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public string Landline { get; set; }
        public string LeadSource { get; set; }
        public string AgencyOrRefferal { get; set; }
        public string CSP_SalesName { get; set; }
        public int? BankID { get; set; }
        public DateOnly IntroductionDate { get; set; } = new DateOnly();
        public int? StatusID { get; set; }
        public DateOnly AccountOpenedOrLoanDisbursedDate { get; set; } = new DateOnly();
        public bool IsPaid { get; set; }
        public string Amount { get; set; }
        public string InvoiceNo { get; set; }
        public List<ContactPerson> ContactPersons { get; set; }
        public string LeadStatus { get; set; }
        public string TradeLicensePath { get; set; }
        public string VATCertificatePath { get; set; }
        public string CurrentUser { get; set; }
        public string Remarks { get; set; }
        public string ExternalAgencyName { get; set; }
        public string ExternalAgentName { get; set; }
        public string ReferralPersonName { get; set; }
        public string ExternalAgencyId { get; set; } 
        public string ExternalAgentId { get; set; }
    }

    public class ContactPerson
    {
        public string Name { get; set; }
        public string Mobile { get; set; }
        public string Designation { get; set; }
        public string Email { get; set; }
        public DateOnly? ContactPersonDOB { get; set; }
    }

    public class LeadDetailModel
    {
        public string ProductRefNo { get; set; }
        public string CompanyName { get; set; }
        public string PrimaryContactPersonName { get; set; }
        public string PrimaryContactPersonMobile { get; set; }
        public string PrimaryContactPersonDesignation { get; set; }
        public string PrimaryContactPersonEmail { get; set; }
        public DateOnly PrimaryContactPersonDOB { get; set; }
        public string Product { get; set; }
        public string LeadSource { get; set; }
        public string AgencyReferral { get; set; }
        public string CSPSales { get; set; }
        public string Bank { get; set; }
        public DateOnly IntroductionDate { get; set; }
        public string Status { get; set; }
        public DateOnly AccountOpenedDate { get; set; }
        public string Paid { get; set; }
        public decimal? Amount { get; set; }
        public string InvoiceNo { get; set; }
        public string Remarks { get; set; }
        public string TradeLicenseNo { get; set; }
        public DateOnly TradeLicenseExpDate { get; set; }
        public string TradeLicensePath { get; set; }
        public string VATCertificatePath { get; set; }
        public string VATTrnNo { get; set; }
        public string ExternalAgencyName { get; set; }
        public string ExternalAgentName { get; set; }
        public string ReferralPersonName { get; set; }
        public string currentUser { get; set; }
        public string activeUserRole { get; set; }


        public List<string> AvailableBanks { get; set; } = new List<string>
    {
        "RAK", "MASHREQ", "WIO", "DIB", "OTHERS"
    };

        public List<string> AvailableStatuses { get; set; } = new List<string>
    {
        "Awaiting Documents", "Submitted", "Pending with client",
        "Accessing with Bank", "Account opened", "Account declined"
    };

    }


    public class AgencyModel
    {
        public long? CustomerOrgId { get; set; }
        public string AgencyName { get; set; }
    }

    public class AgentModel
    {
        public long? NodeId { get; set; }
        public string AgentName { get; set; }
    }
}

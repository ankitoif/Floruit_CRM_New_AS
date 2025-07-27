using System;
using System.Collections.Generic;

namespace OIF.Cams.Data.DAC.AppDbContext;

public partial class TblLead
{
    public int LeadId { get; set; }

    public string? LeadType { get; set; }

    public string? ProductRefNo { get; set; }

    public int? CustomerId { get; set; }

    public string? LeadSource { get; set; }

    public string? AgencyOrReferral { get; set; }

    public string? CspSalesName { get; set; }

    public int? BankId { get; set; }

    public DateOnly? IntroductionDate { get; set; }

    public int? StatusId { get; set; }

    public DateOnly? AccountOpenedOrLoanDisbursedDate { get; set; }

    public bool? Paid { get; set; }

    public decimal? Amount { get; set; }

    public string? InvoiceNo { get; set; }

    public DateTime? CreatedDateTime { get; set; }

    public DateTime? ModifiedDateTime { get; set; }

    public bool? IsValid { get; set; }

    public string? CreatedBy { get; set; }

    public string? ModifiedBy { get; set; }

    public string? ExtAgencyName { get; set; }

    public string? ExtAgentName { get; set; }

    public string? Remarks { get; set; }

    public string? ReferralPersonName { get; set; }

    public string? Risk { get; set; }

    public virtual TblBank? Bank { get; set; }

    public virtual TblCustomer? Customer { get; set; }

    public virtual TblmstProductStatus? Status { get; set; }

    public virtual ICollection<TblLeadAuditLog> TblLeadAuditLogs { get; set; } = new List<TblLeadAuditLog>();

    public virtual ICollection<TblLeadDocument> TblLeadDocuments { get; set; } = new List<TblLeadDocument>();
}

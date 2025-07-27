using System;
using System.Collections.Generic;

namespace OIF.Cams.Data.DAC.AppDbContext;

public partial class TblCustomer
{
    public int CustomerId { get; set; }

    public string? CompanyName { get; set; }

    public string? TradeLicenseNo { get; set; }

    public DateOnly? TradeLicenseExpDate { get; set; }

    public string? VatTrnno { get; set; }

    public string? Address { get; set; }

    public string? LandLineNo { get; set; }

    public bool? IsValid { get; set; }

    public DateTime? CreatedDateTime { get; set; }

    public DateTime? ModifiedDateTime { get; set; }

    public string? CreatedBy { get; set; }

    public string? ModifiedBy { get; set; }

    public string? TradeLicensePath { get; set; }

    public string? VatCertificatePath { get; set; }

    public virtual ICollection<TblContactPerson> TblContactPeople { get; set; } = new List<TblContactPerson>();

    public virtual ICollection<TblCustomerDoc> TblCustomerDocs { get; set; } = new List<TblCustomerDoc>();

    public virtual ICollection<TblLeadDocument> TblLeadDocuments { get; set; } = new List<TblLeadDocument>();

    public virtual ICollection<TblLead> TblLeads { get; set; } = new List<TblLead>();
}

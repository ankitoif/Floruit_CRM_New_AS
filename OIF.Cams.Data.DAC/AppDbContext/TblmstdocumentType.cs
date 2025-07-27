using System;
using System.Collections.Generic;

namespace OIF.Cams.Data.DAC.AppDbContext;

public partial class TblmstdocumentType
{
    public long DocumentTypeId { get; set; }

    public string DocumentName { get; set; } = null!;

    public bool IsValid { get; set; }

    public DateTime CreatedDate { get; set; }

    public virtual ICollection<TblCustomerDoc> TblCustomerDocs { get; set; } = new List<TblCustomerDoc>();

    public virtual ICollection<TblLeadDocument> TblLeadDocuments { get; set; } = new List<TblLeadDocument>();
}

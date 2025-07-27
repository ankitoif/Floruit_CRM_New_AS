using System;
using System.Collections.Generic;

namespace OIF.Cams.Data.DAC.AppDbContext;

public partial class TblLeadDocument
{
    public long LeadDocumentId { get; set; }

    public int LeadId { get; set; }

    public int CustomerId { get; set; }

    public long DocumentTypeId { get; set; }

    public string? DocumentDescription { get; set; }

    public string DocumentPath { get; set; } = null!;

    public bool IsValid { get; set; }

    public DateTime CreatedDateTime { get; set; }

    public DateTime? ModifiedDateTime { get; set; }

    public virtual TblCustomer Customer { get; set; } = null!;

    public virtual TblmstdocumentType DocumentType { get; set; } = null!;

    public virtual TblLead Lead { get; set; } = null!;
}

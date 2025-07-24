using System;
using System.Collections.Generic;

namespace OIF.Cams.Data.DAC.AppDbContext;

public partial class TblMstServiceTypeDocument
{
    public long ServiceTypeDocumentId { get; set; }

    public long? ServiceTypeId { get; set; }

    public string? DocumentCategory { get; set; }

    public string? DocumentType { get; set; }

    public DateTime? CreatedDateTime { get; set; }

    public DateTime? ModifiedDateTime { get; set; }

    public bool? IsValid { get; set; }

    public virtual TblMstServiceType? ServiceType { get; set; }

    public virtual ICollection<TblServiceReqDocumentUpload> TblServiceReqDocumentUploads { get; set; } = new List<TblServiceReqDocumentUpload>();
}

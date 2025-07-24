using System;
using System.Collections.Generic;

namespace OIF.Cams.Data.DAC.AppDbContext;

public partial class TblServiceReqDocumentUpload
{
    public long ServiceReqDocumentUploadId { get; set; }

    public long? ServiceTypeDocumentId { get; set; }

    public string? DocumentUploadFilePath { get; set; }

    public DateTime? CreatedDateTime { get; set; }

    public DateTime? UploadedDateTime { get; set; }

    public bool? IsValid { get; set; }

    public long? ServiceRequestId { get; set; }

    public virtual TblServiceRequest? ServiceRequest { get; set; }

    public virtual TblMstServiceTypeDocument? ServiceTypeDocument { get; set; }
}

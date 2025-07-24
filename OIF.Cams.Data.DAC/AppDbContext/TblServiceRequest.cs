using System;
using System.Collections.Generic;

namespace OIF.Cams.Data.DAC.AppDbContext;

public partial class TblServiceRequest
{
    public long ServiceRequestId { get; set; }

    public long? ServiceTypeId { get; set; }

    public long? Crmid { get; set; }

    public long? CustomerOrgId { get; set; }

    public string? PolicyNo { get; set; }

    public bool? IsPolicyVerified { get; set; }

    public string? ServiceReqNo { get; set; }

    public string? ServiceReqCreatedBy { get; set; }

    public string? PolicyHolderName { get; set; }

    public long? StatusId { get; set; }

    public string? Remarks { get; set; }

    public DateTime? CreatedDateTime { get; set; }

    public DateTime? ModifiedDateTime { get; set; }

    public bool? IsValid { get; set; }

    public virtual TblMstCrm? Crm { get; set; }

    public virtual TblMstCustomerOrg? CustomerOrg { get; set; }

    public virtual TblMstServiceType? ServiceType { get; set; }

    public virtual TblMstServiceReqStatus? Status { get; set; }

    public virtual ICollection<TblServiceReqDocumentUpload> TblServiceReqDocumentUploads { get; set; } = new List<TblServiceReqDocumentUpload>();
}

using System;
using System.Collections.Generic;

namespace OIF.Cams.Data.DAC.AppDbContext;

public partial class TblMstServiceType
{
    public long ServiceTypeId { get; set; }

    public long? CustomerOrgId { get; set; }

    public string? ServiceTypeDescription { get; set; }

    public DateTime? CreatedDateTime { get; set; }

    public DateTime? ModifiedDateTime { get; set; }

    public bool? IsValid { get; set; }

    public virtual TblMstCustomerOrg? CustomerOrg { get; set; }

    public virtual ICollection<TblMstServiceTypeDocument> TblMstServiceTypeDocuments { get; set; } = new List<TblMstServiceTypeDocument>();

    public virtual ICollection<TblServiceRequest> TblServiceRequests { get; set; } = new List<TblServiceRequest>();
}

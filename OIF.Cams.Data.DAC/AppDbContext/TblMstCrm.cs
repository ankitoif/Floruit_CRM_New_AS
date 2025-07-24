using System;
using System.Collections.Generic;

namespace OIF.Cams.Data.DAC.AppDbContext;

public partial class TblMstCrm
{
    public long Crmid { get; set; }

    public long? CustomerOrgId { get; set; }

    public string? Crmname { get; set; }

    public DateTime? CreatedDateTime { get; set; }

    public DateTime? ModifiedDateTime { get; set; }

    public bool? IsValid { get; set; }

    public virtual TblMstCustomerOrg? CustomerOrg { get; set; }

    public virtual ICollection<TblServiceRequest> TblServiceRequests { get; set; } = new List<TblServiceRequest>();
}

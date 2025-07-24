using System;
using System.Collections.Generic;

namespace OIF.Cams.Data.DAC.AppDbContext;

public partial class TblMstCustomerOrg
{
    public long CustomerOrgId { get; set; }

    public string? CustomerOrgName { get; set; }

    public string? CustomerCode { get; set; }

    public string? Logo { get; set; }

    public DateTime? CreatedDateTime { get; set; }

    public DateTime? ModifiedDateTime { get; set; }

    public bool? IsValid { get; set; }

    public virtual ICollection<TblMstBranch> TblMstBranches { get; set; } = new List<TblMstBranch>();

    public virtual ICollection<TblMstCrm> TblMstCrms { get; set; } = new List<TblMstCrm>();

    public virtual ICollection<TblMstServiceType> TblMstServiceTypes { get; set; } = new List<TblMstServiceType>();

    public virtual ICollection<TblServiceRequest> TblServiceRequests { get; set; } = new List<TblServiceRequest>();

    public virtual ICollection<TblUserDetail> TblUserDetails { get; set; } = new List<TblUserDetail>();
}

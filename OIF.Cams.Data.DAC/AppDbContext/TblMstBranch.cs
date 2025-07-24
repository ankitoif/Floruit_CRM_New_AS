using System;
using System.Collections.Generic;

namespace OIF.Cams.Data.DAC.AppDbContext;

public partial class TblMstBranch
{
    public long BranchId { get; set; }

    public long? CustomerOrgId { get; set; }

    public string? BranchName { get; set; }

    public string? BranchCode { get; set; }

    public string? BranchAddress { get; set; }

    public DateTime? CreatedDateTime { get; set; }

    public DateTime? ModifiedDateTime { get; set; }

    public bool? IsValid { get; set; }

    public virtual TblMstCustomerOrg? CustomerOrg { get; set; }

    public virtual ICollection<TblUserDetail> TblUserDetails { get; set; } = new List<TblUserDetail>();
}

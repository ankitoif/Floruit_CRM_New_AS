using System;
using System.Collections.Generic;

namespace OIF.Cams.Data.DAC.AppDbContext;

public partial class TblMstServiceReqStatus
{
    public long ServiceReqStatusId { get; set; }

    public string? Description { get; set; }

    public DateTime? CreatedDateTime { get; set; }

    public bool? IsValid { get; set; }

    public virtual ICollection<TblServiceRequest> TblServiceRequests { get; set; } = new List<TblServiceRequest>();
}

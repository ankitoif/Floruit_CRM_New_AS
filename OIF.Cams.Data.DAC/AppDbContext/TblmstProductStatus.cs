using System;
using System.Collections.Generic;

namespace OIF.Cams.Data.DAC.AppDbContext;

public partial class TblmstProductStatus
{
    public int StatusId { get; set; }

    public string? Description { get; set; }

    public bool? IsValid { get; set; }

    public DateTime? CreatedDateTime { get; set; }

    public DateTime? ModifiedDateTime { get; set; }

    public virtual ICollection<TblLead> TblLeads { get; set; } = new List<TblLead>();
}

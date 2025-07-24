using System;
using System.Collections.Generic;

namespace OIF.Cams.Data.DAC.AppDbContext;

public partial class TblBank
{
    public int BankId { get; set; }

    public string? BankName { get; set; }

    public bool? IsValid { get; set; }

    public DateTime? CreatedDateTime { get; set; }

    public DateTime? ModifiedDateTime { get; set; }

    public virtual ICollection<TblLead> TblLeads { get; set; } = new List<TblLead>();
}

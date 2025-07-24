using System;
using System.Collections.Generic;

namespace OIF.Cams.Data.DAC.AppDbContext;

public partial class TblMstGender
{
    public long GenderId { get; set; }

    public string? Gender { get; set; }

    public bool? IsValid { get; set; }

    public DateTime? CreatedDateTime { get; set; }

    public virtual ICollection<TblUserDetail> TblUserDetails { get; set; } = new List<TblUserDetail>();
}

using System;
using System.Collections.Generic;

namespace OIF.Cams.Data.DAC.AppDbContext;

public partial class TblMstNationality
{
    public long NationalityId { get; set; }

    public string Nationality { get; set; } = null!;

    public bool Isvalid { get; set; }

    public DateTime CreatedDatetime { get; set; }

    public virtual ICollection<TblUserDetail> TblUserDetails { get; set; } = new List<TblUserDetail>();
}

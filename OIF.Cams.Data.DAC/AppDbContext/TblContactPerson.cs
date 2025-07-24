using System;
using System.Collections.Generic;

namespace OIF.Cams.Data.DAC.AppDbContext;

public partial class TblContactPerson
{
    public int ContactId { get; set; }

    public int? CustomerId { get; set; }

    public string? ContactPersonName { get; set; }

    public string? Mobile { get; set; }

    public string? Designation { get; set; }

    public DateTime? CreatedDateTime { get; set; }

    public DateTime? ModifiedDateTime { get; set; }

    public bool? IsValid { get; set; }

    public string? Email { get; set; }

    public DateOnly? Dob { get; set; }

    public virtual TblCustomer? Customer { get; set; }
}

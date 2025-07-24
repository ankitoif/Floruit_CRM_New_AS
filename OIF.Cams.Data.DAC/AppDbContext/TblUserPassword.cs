using System;
using System.Collections.Generic;

namespace OIF.Cams.Data.DAC.AppDbContext;

public partial class TblUserPassword
{
    public decimal Id { get; set; }

    public Guid? UserId { get; set; }

    public string? Password { get; set; }

    public DateOnly? CreatedDateTime { get; set; }

    public DateOnly? ModifiedDateTime { get; set; }
}

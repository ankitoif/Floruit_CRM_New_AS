using System;
using System.Collections.Generic;

namespace OIF.Cams.Data.DAC.AppDbContext;

public partial class TblUserDetail
{
    public long NodeId { get; set; }

    public Guid? UserId { get; set; }

    public string? UserName { get; set; }

    public bool? Status { get; set; }

    public DateTime? CreatedDate { get; set; }

    public Guid? RoleId { get; set; }

    public string? Role { get; set; }

    public string? Salutation { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? EmployeeNumber { get; set; }

    public DateOnly? Dob { get; set; }

    public long? GenderId { get; set; }

    public string? Email { get; set; }

    public string? MobileNo { get; set; }

    public string? FullAddress { get; set; }

    public long? NationalityId { get; set; }

    public DateTime? LastLoginDate { get; set; }

    public long? CustomerOrgId { get; set; }

    public long? BranchId { get; set; }

    public string? AssignUnder { get; set; }

    public virtual TblMstBranch? Branch { get; set; }

    public virtual TblMstCustomerOrg? CustomerOrg { get; set; }

    public virtual TblMstGender? Gender { get; set; }

    public virtual TblMstNationality? Nationality { get; set; }
}

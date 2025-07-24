using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OIF.Cams.Data.DAC.AppDbContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OIF.Cams.Data.DAC
{
    public partial class CamsDbContext : IdentityDbContext<ApplicationUser>
    {
        public CamsDbContext(DbContextOptions<CamsDbContext> options) : base(options) { }

        public virtual DbSet<TblBank> TblBanks { get; set; }

        public virtual DbSet<TblContactPerson> TblContactPeople { get; set; }

        public virtual DbSet<TblCustomer> TblCustomers { get; set; }

        public virtual DbSet<TblLead> TblLeads { get; set; }

        public virtual DbSet<TblLeadAuditLog> TblLeadAuditLogs { get; set; }

        public virtual DbSet<TblMstBranch> TblMstBranches { get; set; }

        public virtual DbSet<TblMstCrm> TblMstCrms { get; set; }

        public virtual DbSet<TblMstCustomerOrg> TblMstCustomerOrgs { get; set; }

        public virtual DbSet<TblMstGender> TblMstGenders { get; set; }

        public virtual DbSet<TblMstNationality> TblMstNationalities { get; set; }

        public virtual DbSet<TblMstServiceReqStatus> TblMstServiceReqStatuses { get; set; }

        public virtual DbSet<TblMstServiceType> TblMstServiceTypes { get; set; }

        public virtual DbSet<TblMstServiceTypeDocument> TblMstServiceTypeDocuments { get; set; }

        public virtual DbSet<TblServiceReqDocumentUpload> TblServiceReqDocumentUploads { get; set; }

        public virtual DbSet<TblServiceRequest> TblServiceRequests { get; set; }

        public virtual DbSet<TblUserDetail> TblUserDetails { get; set; }

        public virtual DbSet<TblUserPassword> TblUserPasswords { get; set; }

        public virtual DbSet<TblmstProductStatus> TblmstProductStatuses { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<TblBank>(entity =>
            {
                entity.HasKey(e => e.BankId).HasName("PK__tblBank__AA08CB33EF808A68");

                entity.ToTable("tblBank");

                entity.Property(e => e.BankId).HasColumnName("BankID");
                entity.Property(e => e.BankName)
                    .HasMaxLength(255)
                    .IsUnicode(false);
                entity.Property(e => e.CreatedDateTime)
                    .HasDefaultValueSql("(getdate())")
                    .HasColumnType("datetime");
                entity.Property(e => e.IsValid).HasDefaultValue(true);
                entity.Property(e => e.ModifiedDateTime)
                    .HasDefaultValueSql("(getdate())")
                    .HasColumnType("datetime");
            });

            modelBuilder.Entity<TblContactPerson>(entity =>
            {
                entity.HasKey(e => e.ContactId).HasName("PK__tblConta__5C6625BB656176D8");

                entity.ToTable("tblContactPerson");

                entity.Property(e => e.ContactId).HasColumnName("ContactID");
                entity.Property(e => e.ContactPersonName)
                    .HasMaxLength(255)
                    .IsUnicode(false);
                entity.Property(e => e.CreatedDateTime)
                    .HasDefaultValueSql("(getdate())")
                    .HasColumnType("datetime");
                entity.Property(e => e.CustomerId).HasColumnName("CustomerID");
                entity.Property(e => e.Designation)
                    .HasMaxLength(100)
                    .IsUnicode(false);
                entity.Property(e => e.Dob).HasColumnName("DOB");
                entity.Property(e => e.Email).HasMaxLength(100);
                entity.Property(e => e.IsValid).HasDefaultValue(true);
                entity.Property(e => e.Mobile)
                    .HasMaxLength(15)
                    .IsUnicode(false);
                entity.Property(e => e.ModifiedDateTime)
                    .HasDefaultValueSql("(getdate())")
                    .HasColumnType("datetime");

                entity.HasOne(d => d.Customer).WithMany(p => p.TblContactPeople)
                    .HasForeignKey(d => d.CustomerId)
                    .HasConstraintName("FK_tblContactPerson_tblCustomers");
            });

            modelBuilder.Entity<TblCustomer>(entity =>
            {
                entity.HasKey(e => e.CustomerId).HasName("PK__tblCusto__A4AE64B819CB1B9C");

                entity.ToTable("tblCustomers");

                entity.Property(e => e.CustomerId).HasColumnName("CustomerID");
                entity.Property(e => e.Address)
                    .HasMaxLength(500)
                    .IsUnicode(false);
                entity.Property(e => e.CompanyName)
                    .HasMaxLength(255)
                    .IsUnicode(false);
                entity.Property(e => e.CreatedBy).HasMaxLength(100);
                entity.Property(e => e.CreatedDateTime)
                    .HasDefaultValueSql("(getdate())")
                    .HasColumnType("datetime");
                entity.Property(e => e.IsValid).HasDefaultValue(true);
                entity.Property(e => e.LandLineNo)
                    .HasMaxLength(50)
                    .IsUnicode(false);
                entity.Property(e => e.ModifiedBy).HasMaxLength(100);
                entity.Property(e => e.ModifiedDateTime)
                    .HasDefaultValueSql("(getdate())")
                    .HasColumnType("datetime");
                entity.Property(e => e.TradeLicenseNo)
                    .HasMaxLength(100)
                    .IsUnicode(false);
                entity.Property(e => e.TradeLicensePath)
                    .HasMaxLength(500)
                    .IsUnicode(false);
                entity.Property(e => e.VatCertificatePath)
                    .HasMaxLength(500)
                    .IsUnicode(false);
                entity.Property(e => e.VatTrnno)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("VatTRNno");
            });

            modelBuilder.Entity<TblLead>(entity =>
            {
                entity.HasKey(e => e.LeadId).HasName("PK__tblLead__73EF791A11055EEF");

                entity.ToTable("tblLead");

                entity.Property(e => e.LeadId).HasColumnName("LeadID");
                entity.Property(e => e.AgencyOrReferral)
                    .HasMaxLength(255)
                    .IsUnicode(false);
                entity.Property(e => e.Amount).HasColumnType("decimal(18, 2)");
                entity.Property(e => e.BankId).HasColumnName("BankID");
                entity.Property(e => e.CreatedBy).HasMaxLength(100);
                entity.Property(e => e.CreatedDateTime)
                    .HasDefaultValueSql("(getdate())")
                    .HasColumnType("datetime");
                entity.Property(e => e.CspSalesName)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("CSP_SalesName");
                entity.Property(e => e.CustomerId).HasColumnName("CustomerID");
                entity.Property(e => e.ExtAgencyName).HasMaxLength(100);
                entity.Property(e => e.ExtAgentName).HasMaxLength(100);
                entity.Property(e => e.InvoiceNo)
                    .HasMaxLength(100)
                    .IsUnicode(false);
                entity.Property(e => e.IsValid).HasDefaultValue(true);
                entity.Property(e => e.LeadSource)
                    .HasMaxLength(100)
                    .IsUnicode(false);
                entity.Property(e => e.LeadType)
                    .HasMaxLength(100)
                    .IsUnicode(false);
                entity.Property(e => e.ModifiedBy).HasMaxLength(100);
                entity.Property(e => e.ModifiedDateTime)
                    .HasDefaultValueSql("(getdate())")
                    .HasColumnType("datetime");
                entity.Property(e => e.ProductRefNo)
                    .HasMaxLength(100)
                    .IsUnicode(false);
                entity.Property(e => e.ReferralPersonName).HasMaxLength(250);
                entity.Property(e => e.Remarks).HasMaxLength(500);
                entity.Property(e => e.StatusId).HasColumnName("StatusID");

                entity.HasOne(d => d.Bank).WithMany(p => p.TblLeads)
                    .HasForeignKey(d => d.BankId)
                    .HasConstraintName("FK_tblLead_tblBank");

                entity.HasOne(d => d.Customer).WithMany(p => p.TblLeads)
                    .HasForeignKey(d => d.CustomerId)
                    .HasConstraintName("FK_tblLead_tblCustomers");

                entity.HasOne(d => d.Status).WithMany(p => p.TblLeads)
                    .HasForeignKey(d => d.StatusId)
                    .HasConstraintName("FK_tblLead_tblmstProductStatus");
            });

            modelBuilder.Entity<TblLeadAuditLog>(entity =>
            {
                entity.HasKey(e => e.AuditLogId).HasName("PK__tblLeadA__EB5F6CDDFF7AB590");

                entity.ToTable("tblLeadAuditLog");

                entity.Property(e => e.AuditLogId).HasColumnName("AuditLogID");
                entity.Property(e => e.CreatedBy).HasMaxLength(100);
                entity.Property(e => e.CreatedDateTime)
                    .HasDefaultValueSql("(getdate())")
                    .HasColumnType("datetime");
                entity.Property(e => e.IsValid).HasDefaultValue(true);
                entity.Property(e => e.LeadId).HasColumnName("LeadID");
                entity.Property(e => e.ModifiedBy).HasMaxLength(100);
                entity.Property(e => e.ModifiedDateTime).HasColumnType("datetime");
                entity.Property(e => e.Remarks).HasMaxLength(500);

                entity.HasOne(d => d.Lead).WithMany(p => p.TblLeadAuditLogs)
                    .HasForeignKey(d => d.LeadId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_tblLeadAuditLog_tbllead");
            });

            modelBuilder.Entity<TblMstBranch>(entity =>
            {
                entity.HasKey(e => e.BranchId).HasName("PK_tblMstBranch_BranchId");

                entity.ToTable("tblMstBranch");

                entity.Property(e => e.BranchAddress).IsUnicode(false);
                entity.Property(e => e.BranchCode)
                    .HasMaxLength(50)
                    .IsUnicode(false);
                entity.Property(e => e.BranchName)
                    .HasMaxLength(100)
                    .IsUnicode(false);
                entity.Property(e => e.CreatedDateTime).HasColumnType("datetime");
                entity.Property(e => e.ModifiedDateTime).HasColumnType("datetime");

                entity.HasOne(d => d.CustomerOrg).WithMany(p => p.TblMstBranches).HasForeignKey(d => d.CustomerOrgId);
            });

            modelBuilder.Entity<TblMstCrm>(entity =>
            {
                entity.HasKey(e => e.Crmid).HasName("PK_tblMstCRM_CRMId");

                entity.ToTable("tblMstCRM");

                entity.Property(e => e.Crmid).HasColumnName("CRMId");
                entity.Property(e => e.CreatedDateTime).HasColumnType("datetime");
                entity.Property(e => e.Crmname)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("CRMName");
                entity.Property(e => e.ModifiedDateTime).HasColumnType("datetime");

                entity.HasOne(d => d.CustomerOrg).WithMany(p => p.TblMstCrms).HasForeignKey(d => d.CustomerOrgId);
            });

            modelBuilder.Entity<TblMstCustomerOrg>(entity =>
            {
                entity.HasKey(e => e.CustomerOrgId).HasName("PK_tblMstCustomerOrg_CustomerOrgId");

                entity.ToTable("tblMstCustomerOrg");

                entity.Property(e => e.CreatedDateTime).HasColumnType("datetime");
                entity.Property(e => e.CustomerCode)
                    .HasMaxLength(50)
                    .IsUnicode(false);
                entity.Property(e => e.CustomerOrgName)
                    .HasMaxLength(100)
                    .IsUnicode(false);
                entity.Property(e => e.Logo)
                    .HasMaxLength(200)
                    .IsUnicode(false);
                entity.Property(e => e.ModifiedDateTime).HasColumnType("datetime");
            });

            modelBuilder.Entity<TblMstGender>(entity =>
            {
                entity.HasKey(e => e.GenderId).HasName("PK_tblMstCountry_GenderId");

                entity.ToTable("tblMstGender");

                entity.Property(e => e.CreatedDateTime).HasColumnType("datetime");
                entity.Property(e => e.Gender)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<TblMstNationality>(entity =>
            {
                entity.HasKey(e => e.NationalityId).HasName("PK_tblMstNationality_NationalityID");

                entity.ToTable("tblMstNationality");

                entity.Property(e => e.NationalityId).HasColumnName("NationalityID");
                entity.Property(e => e.CreatedDatetime).HasColumnType("datetime");
                entity.Property(e => e.Nationality)
                    .HasMaxLength(250)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<TblMstServiceReqStatus>(entity =>
            {
                entity.HasKey(e => e.ServiceReqStatusId).HasName("PK_tblMstServiceReqStatus_ServiceReqStatusId");

                entity.ToTable("tblMstServiceReqStatus");

                entity.Property(e => e.CreatedDateTime).HasColumnType("datetime");
                entity.Property(e => e.Description)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<TblMstServiceType>(entity =>
            {
                entity.HasKey(e => e.ServiceTypeId).HasName("PK_tblMstServiceType_ServiceTypeId");

                entity.ToTable("tblMstServiceType");

                entity.Property(e => e.CreatedDateTime).HasColumnType("datetime");
                entity.Property(e => e.ModifiedDateTime).HasColumnType("datetime");
                entity.Property(e => e.ServiceTypeDescription)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.HasOne(d => d.CustomerOrg).WithMany(p => p.TblMstServiceTypes).HasForeignKey(d => d.CustomerOrgId);
            });

            modelBuilder.Entity<TblMstServiceTypeDocument>(entity =>
            {
                entity.HasKey(e => e.ServiceTypeDocumentId).HasName("PK_tblMstServiceTypeDocument_ServiceTypeDocumentId");

                entity.ToTable("tblMstServiceTypeDocument");

                entity.Property(e => e.CreatedDateTime).HasColumnType("datetime");
                entity.Property(e => e.DocumentCategory)
                    .HasMaxLength(100)
                    .IsUnicode(false);
                entity.Property(e => e.DocumentType)
                    .HasMaxLength(100)
                    .IsUnicode(false);
                entity.Property(e => e.ModifiedDateTime).HasColumnType("datetime");

                entity.HasOne(d => d.ServiceType).WithMany(p => p.TblMstServiceTypeDocuments).HasForeignKey(d => d.ServiceTypeId);
            });

            modelBuilder.Entity<TblServiceReqDocumentUpload>(entity =>
            {
                entity.HasKey(e => e.ServiceReqDocumentUploadId).HasName("PK_tblServiceReqDocumentUpload_ServiceReqDocumentUploadId");

                entity.ToTable("tblServiceReqDocumentUpload");

                entity.Property(e => e.CreatedDateTime).HasColumnType("datetime");
                entity.Property(e => e.DocumentUploadFilePath)
                    .HasMaxLength(200)
                    .IsUnicode(false);
                entity.Property(e => e.UploadedDateTime).HasColumnType("datetime");

                entity.HasOne(d => d.ServiceRequest).WithMany(p => p.TblServiceReqDocumentUploads).HasForeignKey(d => d.ServiceRequestId);

                entity.HasOne(d => d.ServiceTypeDocument).WithMany(p => p.TblServiceReqDocumentUploads).HasForeignKey(d => d.ServiceTypeDocumentId);
            });

            modelBuilder.Entity<TblServiceRequest>(entity =>
            {
                entity.HasKey(e => e.ServiceRequestId).HasName("PK_tblServiceRequest_ServiceRequestId");

                entity.ToTable("tblServiceRequest");

                entity.Property(e => e.CreatedDateTime).HasColumnType("datetime");
                entity.Property(e => e.Crmid).HasColumnName("CRMId");
                entity.Property(e => e.ModifiedDateTime).HasColumnType("datetime");
                entity.Property(e => e.PolicyHolderName)
                    .HasMaxLength(200)
                    .IsUnicode(false);
                entity.Property(e => e.PolicyNo)
                    .HasMaxLength(50)
                    .IsUnicode(false);
                entity.Property(e => e.Remarks)
                    .HasMaxLength(200)
                    .IsUnicode(false);
                entity.Property(e => e.ServiceReqCreatedBy)
                    .HasMaxLength(100)
                    .IsUnicode(false);
                entity.Property(e => e.ServiceReqNo)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.HasOne(d => d.Crm).WithMany(p => p.TblServiceRequests).HasForeignKey(d => d.Crmid);

                entity.HasOne(d => d.CustomerOrg).WithMany(p => p.TblServiceRequests).HasForeignKey(d => d.CustomerOrgId);

                entity.HasOne(d => d.ServiceType).WithMany(p => p.TblServiceRequests).HasForeignKey(d => d.ServiceTypeId);

                entity.HasOne(d => d.Status).WithMany(p => p.TblServiceRequests)
                    .HasForeignKey(d => d.StatusId)
                    .HasConstraintName("FK_tblServiceRequest_StatusId_tblMstServiceReqStatus_ServiceReqStatusId");
            });

            modelBuilder.Entity<TblUserDetail>(entity =>
            {
                entity.HasKey(e => e.NodeId).HasName("PK_tblUserDetails_NodeID");

                entity.ToTable("tblUserDetails");

                entity.Property(e => e.NodeId).HasColumnName("NodeID");
                entity.Property(e => e.AssignUnder)
                    .HasMaxLength(100)
                    .IsUnicode(false);
                entity.Property(e => e.CreatedDate).HasColumnType("datetime");
                entity.Property(e => e.Dob).HasColumnName("DOB");
                entity.Property(e => e.Email)
                    .HasMaxLength(100)
                    .IsUnicode(false);
                entity.Property(e => e.EmployeeNumber)
                    .HasMaxLength(255)
                    .IsUnicode(false);
                entity.Property(e => e.FirstName)
                    .HasMaxLength(100)
                    .IsUnicode(false);
                entity.Property(e => e.FullAddress).IsUnicode(false);
                entity.Property(e => e.LastLoginDate).HasColumnType("datetime");
                entity.Property(e => e.LastName)
                    .HasMaxLength(100)
                    .IsUnicode(false);
                entity.Property(e => e.MobileNo)
                    .HasMaxLength(50)
                    .IsUnicode(false);
                entity.Property(e => e.NationalityId).HasColumnName("NationalityID");
                entity.Property(e => e.Role)
                    .HasMaxLength(50)
                    .IsUnicode(false);
                entity.Property(e => e.Salutation)
                    .HasMaxLength(15)
                    .IsUnicode(false);
                entity.Property(e => e.UserId).HasColumnName("UserID");
                entity.Property(e => e.UserName)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.HasOne(d => d.Branch).WithMany(p => p.TblUserDetails)
                    .HasForeignKey(d => d.BranchId)
                    .HasConstraintName("Fk_tblUserDetails_tblMstBranch_BranchId");

                entity.HasOne(d => d.CustomerOrg).WithMany(p => p.TblUserDetails)
                    .HasForeignKey(d => d.CustomerOrgId)
                    .HasConstraintName("Fk_tblUserDetails_tblMstCustomerOrg_CustomerOrgId");

                entity.HasOne(d => d.Gender).WithMany(p => p.TblUserDetails)
                    .HasForeignKey(d => d.GenderId)
                    .HasConstraintName("Fk_tblUserDetails_tblMstGender_GenderId");

                entity.HasOne(d => d.Nationality).WithMany(p => p.TblUserDetails)
                    .HasForeignKey(d => d.NationalityId)
                    .HasConstraintName("Fk_tblUserDetails_tblMstNationality_NationalityID");
            });

            modelBuilder.Entity<TblUserPassword>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK_tblUserPassword_Id");

                entity.ToTable("tblUserPassword");

                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd()
                    .HasColumnType("numeric(18, 0)");
                entity.Property(e => e.Password).IsUnicode(false);
                entity.Property(e => e.UserId).HasColumnName("UserID");
            });

            modelBuilder.Entity<TblmstProductStatus>(entity =>
            {
                entity.HasKey(e => e.StatusId).HasName("PK__tblmstPr__C8EE204383F12C54");

                entity.ToTable("tblmstProductStatus");

                entity.Property(e => e.StatusId).HasColumnName("StatusID");
                entity.Property(e => e.CreatedDateTime)
                    .HasDefaultValueSql("(getdate())")
                    .HasColumnType("datetime");
                entity.Property(e => e.Description)
                    .HasMaxLength(255)
                    .IsUnicode(false);
                entity.Property(e => e.IsValid).HasDefaultValue(true);
                entity.Property(e => e.ModifiedDateTime)
                    .HasDefaultValueSql("(getdate())")
                    .HasColumnType("datetime");
            });

            OnModelCreatingPartial(modelBuilder);
        }
        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}

using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OIF.Cams.Data.DAC;
using OIF.Cams.Data.DAC.AppDbContext;
using OIF.Cams.Model.LeadVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OIF.Cams.Data.Repository.Lead
{
    public class LeadRepo : ILeadRepo
    {
        private readonly CamsDbContext _context;
        private readonly ILogger<ILeadRepo> _ILogger;
        public LeadRepo(CamsDbContext context, ILogger<ILeadRepo> ilogger)
        {
            _context = context;
            _ILogger = ilogger;
        }
        public async Task<bool> CreateLeadAsync(LeadViewModel model)
        {
            try
            {
                string tradeLicensePath = null;
                string vatCertificatePath = null;

                var uploadsRootFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");

                // Ensure the uploads folder exists
                if (!Directory.Exists(uploadsRootFolder))
                {
                    Directory.CreateDirectory(uploadsRootFolder);
                }

                // Save Trade License file
                if (model.TradeLicense != null && model.TradeLicense.Length > 0)
                {
                    var fileName = $"TradeLicense_{Guid.NewGuid()}{Path.GetExtension(model.TradeLicense.FileName)}";
                    var filePath = Path.Combine(uploadsRootFolder, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.TradeLicense.CopyToAsync(stream);
                    }

                    tradeLicensePath = $"uploads/{fileName}"; // relative path to be stored in DB
                }

                // Save VAT Certificate file
                if (model.VATCertificate != null && model.VATCertificate.Length > 0)
                {
                    var fileName = $"VATCertificate_{Guid.NewGuid()}{Path.GetExtension(model.VATCertificate.FileName)}";
                    var filePath = Path.Combine(uploadsRootFolder, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.VATCertificate.CopyToAsync(stream);
                    }

                    vatCertificatePath = $"uploads/{fileName}";
                }

                TblCustomer cust = new TblCustomer();
                await _context.TblCustomers.AddAsync(cust);
                cust.CompanyName = model.CompanyName;
                cust.TradeLicenseNo = model.TradeLicenseNumber;
                cust.TradeLicenseExpDate = model.TradeLicenseExpiry;
                cust.VatTrnno = model.VAT;
                cust.Address = model.Address;
                cust.LandLineNo = model.Landline;
                cust.IsValid = true;
                cust.CreatedDateTime = DateTime.Now;
                cust.ModifiedDateTime = DateTime.Now;
                cust.CreatedBy = model.CurrentUser;
                cust.TradeLicensePath = tradeLicensePath;
                cust.VatCertificatePath = vatCertificatePath;

                _context.TblCustomers.Add(cust);
                await _context.SaveChangesAsync();


                if (model.ContactPersons.Count > 0)
                {
                    foreach (var item in model.ContactPersons)
                    {
                        TblContactPerson contactDetails = new TblContactPerson();

                        contactDetails.CustomerId = cust.CustomerId;
                        contactDetails.ContactPersonName = item.Name;
                        contactDetails.Mobile = item.Mobile;
                        contactDetails.Email = item.Email;
                        contactDetails.Designation = item.Designation;
                        contactDetails.Dob = item.ContactPersonDOB;
                        contactDetails.CreatedDateTime = DateTime.Now;
                        contactDetails.ModifiedDateTime = DateTime.Now;
                        contactDetails.IsValid = true;

                        _context.TblContactPeople.Add(contactDetails);
                        await _context.SaveChangesAsync();
                    }
                }

                TblLead lead = new TblLead();
                lead.LeadType = model.Product;
                // 1. Define the prefix based on product
                string prefix = "";
                if (model.Product == "Account")
                    prefix = "AC";
                else if (model.Product == "Asset")
                    prefix = "AS";
                else
                    prefix = "XX"; // fallback/default

                // 2. Get the last ProductRefNo from DB (with same prefix)
                string lastRefNo = _context.TblLeads.Where(l => l.ProductRefNo.StartsWith(prefix)).OrderByDescending(l => l.ProductRefNo).Select(l => l.ProductRefNo).FirstOrDefault();

                int newNumber = 1;

                if (!string.IsNullOrEmpty(lastRefNo))
                {
                    // Extract numeric part and increment
                    string numberPart = lastRefNo.Substring(prefix.Length); // e.g., "0002"
                    if (int.TryParse(numberPart, out int lastNumber))
                    {
                        newNumber = lastNumber + 1;
                    }
                }

                // 3. Format new ProductRefNo with prefix + padded number
                string newRefNo = prefix + newNumber.ToString("D4"); // e.g., "AC0003"

                long agencyId = Convert.ToInt64(model.ExternalAgencyName);
                var ExternalAgencyName = await _context.TblMstBranches.AsNoTracking().Where(x => x.BranchId == agencyId).Select(x => x.BranchName).FirstOrDefaultAsync();
                long agentNodeId = Convert.ToInt64(model.ExternalAgentName);
                var ExternalAgentName = await _context.TblUserDetails.AsNoTracking().Where(x => x.NodeId == agentNodeId).Select(x => x.FirstName + " " + x.LastName).FirstOrDefaultAsync();

                // 4. Assign to lead
                lead.ProductRefNo = newRefNo;
                lead.CustomerId = cust.CustomerId;
                lead.LeadSource = model.LeadSource;
                lead.AgencyOrReferral = model.AgencyOrRefferal;
                lead.CspSalesName = model.CSP_SalesName;
                lead.BankId = model.BankID != 0 ? model.BankID : null;
                lead.IntroductionDate = model.IntroductionDate;
                lead.StatusId = model.StatusID != 0 ? model.StatusID : null;
                lead.AccountOpenedOrLoanDisbursedDate = model.AccountOpenedOrLoanDisbursedDate;
                lead.Paid = model.IsPaid;
                lead.Amount = Convert.ToDecimal(model.Amount);
                lead.InvoiceNo = model.InvoiceNo;
                lead.CreatedDateTime = DateTime.Now;
                lead.ModifiedDateTime = DateTime.Now;
                lead.IsValid = true;
                lead.CreatedBy = model.CurrentUser;
                lead.StatusId = 2;              
                lead.ExtAgencyName = ExternalAgencyName;
                lead.ExtAgentName = ExternalAgentName;
                lead.ReferralPersonName = model.ReferralPersonName;
                if (model.LeadSource == "Agency")
                {
                    lead.AgencyOrReferral = "Agency";
                }
                if (model.LeadSource == "Referral")
                {
                    lead.AgencyOrReferral = "Referral";
                }
                lead.Remarks = model.Remarks;
                lead.CustomerId = cust.CustomerId;
                await _context.TblLeads.AddAsync(lead);
                await _context.SaveChangesAsync();

                // Create an audit log entry for the lead for remarks
                TblLeadAuditLog leadAuditLog = new TblLeadAuditLog();
                leadAuditLog.LeadId = lead.LeadId;
                leadAuditLog.Remarks = lead.Remarks;
                leadAuditLog.CreatedBy = model.CurrentUser;
                leadAuditLog.CreatedDateTime = DateTime.Now;
                leadAuditLog.IsValid = true;
                await _context.TblLeadAuditLogs.AddAsync(leadAuditLog);
                await _context.SaveChangesAsync();


                //For Saving data to tblCustomerDoc

                // 2. Load document types from the DB
                var docTypes = await _context.TblmstdocumentTypes.ToListAsync();

                // 3. Map document names to model files
                var fileMap = new Dictionary<string, IFormFile>(StringComparer.OrdinalIgnoreCase)
                {
                    ["Trade License"] = model.TradeLicense,
                    ["Memorandum of Association (MOA)"] = model.MOA,
                    ["Tenancy Contract"] = model.TenancyContract,
                    ["Share Certificate"] = model.ShareCertificate,
                    ["Certificate of Incorporation / Certificate of Formation"] = model.CertificateOfIncorporation,
                    ["KYC"] = model.KYC,
                    ["Passport Front"] = model.PassportFront,
                    ["Passport Back"] = model.PassportBack,
                    ["EmiratesID Front"] = model.EmiratesIDFront,
                    ["EmiratesID Back"] = model.EmiratesIDBack,
                    ["Bank Statement"] = model.BankStatement,
                    ["Others Documents"] = null, // handle manually if needed
                    ["VAT Certificate"] = model.VATCertificate
                };

                var customerDocs = new List<TblCustomerDoc>();

                foreach (var docType in docTypes)
                {
                    if (fileMap.TryGetValue(docType.DocumentName.Trim(), out var formFile) && formFile != null && formFile.Length > 0)
                    {
                        var fileName = $"{docType.DocumentName.Replace(" ", "_")}_{Guid.NewGuid()}{Path.GetExtension(formFile.FileName)}";
                        var filePath = Path.Combine(uploadsRootFolder, fileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await formFile.CopyToAsync(stream);
                        }

                        customerDocs.Add(new TblCustomerDoc
                        {
                            CustomerId = cust.CustomerId,
                            DocumentTypeId = docType.DocumentTypeId,
                            DocumentDescription = docType.DocumentName,
                            DocumentPath = $"uploads/{fileName}",
                            IsValid = true,
                            CreatedDateTime = DateTime.Now
                        });
                    }
                }

                // 4. Save documents to DB
                if (customerDocs.Any())
                {
                    await _context.TblCustomerDocs.AddRangeAsync(customerDocs);
                    await _context.SaveChangesAsync();
                }


                return true;
            }
            catch (Exception ex)
            {
                // Log the exception if required
                return false;
            }
        }
        public async Task<List<LeadViewGridData>> GetLeadDashboard(string currentUser, bool isAdmin)
        {
            List<LeadViewGridData> leads = new List<LeadViewGridData>();

            try
            {
                var CurrentUserName = await _context.TblUserDetails
                        .Where(u => u.Email == currentUser)
                        .Select(u => new
                        {
                            u.FirstName,
                            u.LastName
                        })
                        .FirstOrDefaultAsync();
                string CurrentUserfullName = CurrentUserName != null ? $"{CurrentUserName.FirstName} {CurrentUserName.LastName}" : "Unknown User";

                var leadsData = await (from a in _context.TblLeads.AsNoTracking()
                                       join b in _context.TblCustomers.AsNoTracking() on a.CustomerId equals b.CustomerId
                                       where (a.CreatedBy.ToLower() == currentUser.ToLower() || isAdmin == true) && a.IsValid == true
                                       select new
                                       {
                                           Lead = a,
                                           Customer = b
                                       }).ToListAsync();

                if (leadsData.Count > 0)
                {
                    foreach (var item in leadsData)
                    {
                        LeadViewGridData model = new LeadViewGridData();
                        model.LeadID = item.Lead.LeadId;
                        model.LeadType = item.Lead.LeadType;
                        model.ProductRefNo = item.Lead.ProductRefNo;
                        model.CompanyName = item.Customer.CompanyName;
                        model.TradeLicenseNo = item.Customer.TradeLicenseNo;
                        model.Address = item.Customer.Address;
                        model.TradeLicenseExpDate = item.Customer.TradeLicenseExpDate.HasValue ? item.Customer.TradeLicenseExpDate.Value : default(DateOnly);
                        model.VatTRNno = item.Customer.VatTrnno;
                        model.LandLineNo = item.Customer.LandLineNo;
                        model.LeadSource = item.Lead.LeadSource;
                        model.AgencyOrReferral = item.Lead.AgencyOrReferral;
                        model.CSP_SalesName = item.Lead.CspSalesName;
                        model.BankID = item.Lead.BankId.HasValue ? item.Lead.BankId.Value : 0;
                        model.IntroductionDate = item.Lead.IntroductionDate.HasValue ? item.Lead.IntroductionDate.Value : null;
                        model.StatusID = item.Lead.StatusId.HasValue ? item.Lead.StatusId.Value : 0;
                        model.AccountOpenedOrLoanDisbursedDate = item.Lead.AccountOpenedOrLoanDisbursedDate.HasValue ? item.Lead.AccountOpenedOrLoanDisbursedDate.Value : null;
                        model.Paid = item.Lead.Paid.HasValue ? item.Lead.Paid.Value : false;
                        model.Amount = item.Lead.Amount.HasValue ? item.Lead.Amount.Value : 0;
                        model.InvoiceNo = item.Lead.InvoiceNo;
                        model.CreatedDateTime = item.Lead.CreatedDateTime.HasValue ? item.Lead.CreatedDateTime.Value : DateTime.Now;
                        model.ModifiedDateTime = item.Lead.ModifiedDateTime.HasValue ? item.Lead.ModifiedDateTime.Value : null;
                        model.IsValid = item.Lead.IsValid.HasValue ? item.Lead.IsValid.Value : false;
                        model.CreatedBy = item.Lead.CreatedBy;
                        model.ModifiedBy = item.Lead.ModifiedBy;
                        model.CurrentUserFullName = CurrentUserfullName;


                        leads.Add(model);
                    }
                }

            }
            catch (Exception ex)
            {
                _ILogger.LogError(ex, "Error occurred while fetching service request dashboard data.");
                return new List<LeadViewGridData>();
            }
            return leads;
        }

        public async Task<LeadViewModel> GetLeadByTradeLicense(string tradeLicenseNumber)
        {
            try
            {
                var result = await (
                    from a in _context.TblLeads.AsNoTracking()
                    join b in _context.TblCustomers.AsNoTracking() on a.CustomerId equals b.CustomerId
                    join c in _context.TblContactPeople.AsNoTracking() on b.CustomerId equals c.CustomerId
                    join d in _context.TblmstProductStatuses.AsNoTracking() on a.StatusId equals d.StatusId
                    where b.TradeLicenseNo == tradeLicenseNumber && a.IsValid == true
                    orderby a.CreatedDateTime descending

                    select new LeadViewModel
                    {
                        Product = a.LeadType, // If there's a dedicated Product field, replace this
                        CompanyName = b.CompanyName,
                        TradeLicenseNumber = b.TradeLicenseNo,
                        TradeLicenseExpiry = b.TradeLicenseExpDate.HasValue ? b.TradeLicenseExpDate.Value : new DateOnly(),
                        vatTrn = b.VatTrnno,
                        Address = b.Address,
                        Landline = b.LandLineNo,
                        LeadSource = a.LeadSource,
                        AgencyOrRefferal = a.AgencyOrReferral,
                        CSP_SalesName = a.CspSalesName,
                        BankID = a.BankId,
                        IntroductionDate = a.IntroductionDate.HasValue ? a.IntroductionDate.Value : new DateOnly(),
                        StatusID = a.StatusId,
                        LeadStatus = d.Description,
                        AccountOpenedOrLoanDisbursedDate = a.AccountOpenedOrLoanDisbursedDate.HasValue ? a.AccountOpenedOrLoanDisbursedDate.Value : new DateOnly(),
                        IsPaid = (bool)a.Paid,
                        Amount = a.Amount.HasValue ? a.Amount.Value.ToString("0.00") : null,
                        InvoiceNo = a.InvoiceNo,
                        TradeLicensePath = b.TradeLicensePath,
                        VATCertificatePath = b.VatCertificatePath,


                        ContactPersons = _context.TblContactPeople
                                      .AsNoTracking()
                                      .Where(cp => cp.CustomerId == b.CustomerId && (bool)cp.IsValid)
                                      .Select(cp => new ContactPerson
                                      {
                                          Name = cp.ContactPersonName,
                                          Mobile = cp.Mobile,
                                          Designation = cp.Designation,
                                          Email = cp.Email,
                                          ContactPersonDOB = cp.Dob
                                          
                                      }).ToList(),

                    }
                ).FirstOrDefaultAsync();

                return result;
            }
            catch (Exception)
            {

                throw;
            }
        }
        public async Task<LeadDetailModel> GetLeadByProductRefNo(string productRefNo)
        {
            try
            {
                LeadDetailModel leadData = new LeadDetailModel();
                if (productRefNo != null)
                {
                    var rawData = await (from a in _context.TblLeads.AsNoTracking()
                                         join b in _context.TblCustomers.AsNoTracking() on a.CustomerId equals b.CustomerId
                                         //join c in _context.TblBanks.AsNoTracking() on a.BankId equals c.BankId
                                         join c in _context.TblBanks.AsNoTracking() on a.BankId equals c.BankId into bankJoin
                                         from c in bankJoin.DefaultIfEmpty()
                                         join d in _context.TblmstProductStatuses.AsNoTracking() on a.StatusId equals d.StatusId
                                         join e in _context.TblContactPeople.AsNoTracking() on a.CustomerId equals e.CustomerId
                                         where a.ProductRefNo == productRefNo && a.IsValid == true
                                         select new
                                         {
                                             a.ProductRefNo,
                                             a.LeadType,
                                             b.CompanyName,
                                             b.TradeLicenseNo,
                                             b.TradeLicenseExpDate,
                                             a.LeadSource,
                                             a.AgencyOrReferral,
                                             a.CspSalesName,
                                             BankName = c != null ? c.BankName : null,
                                             a.IntroductionDate,
                                             d.Description,
                                             a.AccountOpenedOrLoanDisbursedDate,
                                             a.Paid,
                                             a.Amount,
                                             a.InvoiceNo,
                                             a.Remarks,
                                             b.TradeLicensePath,
                                             b.VatCertificatePath,
                                             b.VatTrnno,
                                             a.ExtAgencyName,
                                             a.ExtAgentName,
                                             a.ReferralPersonName,
                                             e.ContactPersonName,
                                             e.Mobile,
                                             e.Designation,
                                             e.Email,
                                             e.Dob,

                                             a.CreatedBy
                                         }).FirstOrDefaultAsync();

                    if (rawData != null)
                    {
                        leadData = new LeadDetailModel
                        {
                            ProductRefNo = rawData.ProductRefNo,
                            Product = rawData.LeadType,
                            CompanyName = rawData.CompanyName,
                            TradeLicenseNo = rawData.TradeLicenseNo,
                            TradeLicenseExpDate = rawData.TradeLicenseExpDate.HasValue ? rawData.TradeLicenseExpDate.Value : DateOnly.MinValue,
                            LeadSource = rawData.LeadSource,
                            AgencyReferral = rawData.AgencyOrReferral,
                            CSPSales = rawData.CreatedBy,
                            Bank = rawData.BankName,
                            IntroductionDate = rawData.IntroductionDate.HasValue ? rawData.IntroductionDate.Value : DateOnly.MinValue,
                            Status = rawData.Description,
                            AccountOpenedDate = rawData.AccountOpenedOrLoanDisbursedDate.HasValue ? rawData.AccountOpenedOrLoanDisbursedDate.Value : DateOnly.MinValue,
                            Paid = rawData.Paid.HasValue ? (rawData.Paid.Value ? "Yes" : "No") : "No",
                            Amount = rawData.Amount ?? 0,
                            InvoiceNo = rawData.InvoiceNo,
                            Remarks = rawData.Remarks,
                            TradeLicensePath = rawData.TradeLicensePath,
                            VATCertificatePath = rawData.VatCertificatePath,
                            ExternalAgencyName = rawData.ExtAgencyName,
                            ExternalAgentName = rawData.ExtAgentName,
                            ReferralPersonName = rawData.ReferralPersonName,
                            VATTrnNo = rawData.VatTrnno,
                            PrimaryContactPersonName = rawData.ContactPersonName,
                            PrimaryContactPersonMobile = rawData.Mobile,
                            PrimaryContactPersonDesignation = rawData.Designation,
                            PrimaryContactPersonEmail = rawData.Email,
                            PrimaryContactPersonDOB = rawData.Dob.HasValue ? rawData.Dob.Value : DateOnly.MinValue
                        };
                    }
                }
                return leadData;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<bool> UpdateLeadDetails(LeadDetailModel model)
        {
            try
            {
                var existingLead = await _context.TblLeads.AsNoTracking().FirstOrDefaultAsync(x => x.ProductRefNo == model.ProductRefNo);
                var BankID = await _context.TblBanks.AsNoTracking().Where(x => x.BankName == model.Bank).Select(x => x.BankId).FirstOrDefaultAsync();
                var statusID = await _context.TblmstProductStatuses.AsNoTracking().Where(x => x.Description == model.Status).Select(x => x.StatusId).FirstOrDefaultAsync();

                if (existingLead == null)
                {
                    throw new Exception("Lead not found.");
                }

                if (existingLead != null)
                {
                    if (model.activeUserRole == "Operations")
                    {
                        existingLead.StatusId = statusID;
                        existingLead.BankId = BankID;
                        existingLead.AccountOpenedOrLoanDisbursedDate = model.AccountOpenedDate;
                        existingLead.Paid = string.Equals(model.Paid, "Yes", StringComparison.OrdinalIgnoreCase);
                        existingLead.Amount = model.Amount;
                        existingLead.InvoiceNo = model.InvoiceNo;
                        existingLead.ModifiedDateTime = DateTime.Now;
                        existingLead.ModifiedBy = model.currentUser;
                        existingLead.Risk = model.Risk;
                        

                        existingLead.Remarks = model.Remarks;
                        _context.TblLeads.Update(existingLead);
                    }
                    if (model.activeUserRole == "Agent")
                    {
                        existingLead.CspSalesName = model.CSPSales;
                        existingLead.BankId = BankID;
                        existingLead.IntroductionDate = model.IntroductionDate;
                        existingLead.AccountOpenedOrLoanDisbursedDate = model.AccountOpenedDate;
                        existingLead.Paid = string.Equals(model.Paid, "Yes", StringComparison.OrdinalIgnoreCase);
                        existingLead.Amount = model.Amount;
                        existingLead.InvoiceNo = model.InvoiceNo;
                        existingLead.ModifiedDateTime = DateTime.Now;
                        existingLead.ModifiedBy = model.currentUser;
                        existingLead.Remarks = model.Remarks;
                        existingLead.Risk = model.Risk;
                        _context.TblLeads.Update(existingLead);
                    }

                    var newRemark = new TblLeadAuditLog
                    {
                        LeadId = existingLead.LeadId,
                        Remarks = model.Remarks,
                        ModifiedBy = model.currentUser,
                        ModifiedDateTime = DateTime.Now
                    };
                    await _context.TblLeadAuditLogs.AddAsync(newRemark);



                    // Save uploaded documents if any
                    var uploadsRootFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "lead-docs");
                    // Ensure the uploads/lead-docs folder exists
                    if (!Directory.Exists(uploadsRootFolder))
                    {
                        Directory.CreateDirectory(uploadsRootFolder);
                    }

                    if (model.AdditionalDocuments != null && model.AdditionalDocuments.Any())
                    {
                        foreach (var doc in model.AdditionalDocuments)
                        {
                            if (doc.File != null && doc.File.Length > 0)
                            {
                                string uniqueFileName = $"{Guid.NewGuid()}{Path.GetExtension(doc.File.FileName)}";
                                string filePath = Path.Combine(uploadsRootFolder, uniqueFileName);

                                using (var fileStream = new FileStream(filePath, FileMode.Create))
                                {
                                    await doc.File.CopyToAsync(fileStream);
                                }

                                var docEntity = new TblLeadDocument
                                {
                                    LeadId = existingLead.LeadId,
                                    CustomerId = existingLead.CustomerId ?? 0,
                                    DocumentTypeId = await _context.TblmstdocumentTypes
                                        .Where(d => d.DocumentName == doc.DocumentType)
                                        .Select(d => d.DocumentTypeId)
                                        .FirstOrDefaultAsync(),

                                    DocumentDescription = doc.Description,
                                    DocumentPath = "/uploads/lead-docs/" + uniqueFileName,
                                    IsValid = true,
                                    CreatedDateTime = DateTime.Now,
                                    ModifiedDateTime = DateTime.Now
                                };

                                _context.TblLeadDocuments.Add(docEntity);
                            }
                        }

                        await _context.SaveChangesAsync();
                    }





                    await _context.SaveChangesAsync();
                    return true;
                }
                return false;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<List<AgencyModel>> GetAllAgencies()
        {
            try
            {
                var result = await (
                           from a in _context.TblMstBranches.AsNoTracking()
                           join b in _context.TblMstCustomerOrgs.AsNoTracking()
                               on a.CustomerOrgId equals b.CustomerOrgId
                           where b.CustomerOrgName == "External Agency"
                           select new AgencyModel
                           {
                               CustomerOrgId = a.BranchId,
                               AgencyName = a.BranchName
                           }).ToListAsync();

                return result;
            }
            catch (Exception ex)
            {
                _ILogger.LogError(ex, "Error occurred while fetching Agencies");
                return new List<AgencyModel>();
            }
        }

        public async Task<List<AgentModel>> GetAgentsByAgency(int agencyId)
        {
            try
            {
                var result = await (
                          from a in _context.TblUserDetails.AsNoTracking()
                          join b in _context.TblMstCustomerOrgs.AsNoTracking()
                              on a.CustomerOrgId equals b.CustomerOrgId
                          where b.CustomerOrgName == "External Agency" && agencyId == a.BranchId
                          select new AgentModel
                          {
                              NodeId = a.NodeId,
                              AgentName = a.FirstName + " " + a.LastName,
                          }).ToListAsync();

                return result;
            }
            catch (Exception ex)
            {
                _ILogger.LogError(ex, "Error occurred while fetching Agencies");
                return new List<AgentModel>();
            }
        }


     }
}

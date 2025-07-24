using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OIF.Cams.CrossCutting;
using OIF.Cams.Data.DAC;
using OIF.Cams.Data.DAC.AppDbContext;
using OIF.Cams.Model.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace OIF.Cams.Data.Repository.Account
{
    public class AccountRepo : IAccountRepo
    {
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly ILogger<AccountRepo> logger;
        private readonly CamsDbContext Context;
        private readonly IHttpContextAccessor httpContextAccessor;
        private const int MaxResetAttempts = 3;

        public AccountRepo(ILogger<AccountRepo> _logger, CamsDbContext _context, SignInManager<ApplicationUser> _signInManager, UserManager<ApplicationUser> _userManager, RoleManager<IdentityRole> _roleManager, IHttpContextAccessor _httpContextAccessor)
        {
            logger = _logger;
            Context = _context;
            signInManager = _signInManager;
            userManager = _userManager;
            roleManager = _roleManager;
            httpContextAccessor = _httpContextAccessor;
        }

        public async Task<bool> CheckPasswordAsync(ApplicationUser user, string password)
        {
            try
            {
                if (user != null && await userManager.CheckPasswordAsync(user, password))
                {
                    var roles = await userManager.GetRolesAsync(user);

                    var userClaims = await userManager.GetClaimsAsync(user);
                    var existingClaim = userClaims.FirstOrDefault(c => c.Type == "ActiveRole");
                    string activeRole = string.Empty;

                    if (existingClaim != null && roles.Contains(existingClaim.Value))
                    {
                        activeRole = existingClaim.Value;
                    }
                    else
                    {
                        activeRole = roles.FirstOrDefault();

                        if (existingClaim != null)
                            await userManager.RemoveClaimAsync(user, existingClaim);

                        if (!string.IsNullOrEmpty(activeRole))
                            await userManager.AddClaimAsync(user, new Claim("ActiveRole", activeRole));
                    }
                    httpContextAccessor.HttpContext.Session.SetString("ActiveRole", activeRole);

                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, CrossCutting_Constants.RepositoryPolicy);
                return false;
            }
        }
        public async Task SignInAsync(ApplicationUser user, bool rememberMe)
        {
            try
            {
                // Sign in with updated claims
                var authProps = new AuthenticationProperties
                {
                    IsPersistent = false,
                    AllowRefresh = false
                };
                await signInManager.SignInAsync(user, authProps);

                httpContextAccessor.HttpContext.Session.SetString("UserName", user.UserName);
                httpContextAccessor.HttpContext.Session.SetString("UserId", user.Id);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, CrossCutting_Constants.RepositoryPolicy);
            }
        }
        public async Task<SignInResult> PasswordSignInAsync(string userName, string password, bool rememberMe)
        {
            try
            {
                var result = await signInManager.PasswordSignInAsync(userName, password, rememberMe, lockoutOnFailure: true);

                if (result.Succeeded)
                {
                    var user = await userManager.FindByEmailAsync(userName);
                    var roles = await userManager.GetRolesAsync(user);
                    string activeRole = string.Empty;

                    var passwordExpiryLimit = 90;
                    if (user.LastPasswordChangedDate != null)
                    {
                        if (user.LastPasswordChangedDate.HasValue && user.LastPasswordChangedDate.Value.AddDays(passwordExpiryLimit) < DateTime.UtcNow)
                        {
                            httpContextAccessor.HttpContext.Session.SetString("PasswordExpired", "true");
                        }
                    }

                    var userClaims = await userManager.GetClaimsAsync(user);
                    var existingClaim = userClaims.FirstOrDefault(c => c.Type == "ActiveRole");

                    if (existingClaim != null && roles.Contains(existingClaim.Value))
                    {
                        activeRole = existingClaim.Value;
                    }
                    else
                    {
                        activeRole = roles.FirstOrDefault();

                        if (existingClaim != null)
                            await userManager.RemoveClaimAsync(user, existingClaim);

                        if (!string.IsNullOrEmpty(activeRole))
                            await userManager.AddClaimAsync(user, new Claim("ActiveRole", activeRole));
                    }

                    var userDetails = await Context.TblUserDetails.FirstOrDefaultAsync(x => x.UserId == Guid.Parse(user.Id));
                    if (userDetails != null)
                    {
                        userDetails.LastLoginDate = DateTime.Now;
                        await Context.SaveChangesAsync();
                    }

                    httpContextAccessor.HttpContext.Session.SetString("ActiveRole", activeRole);
                    httpContextAccessor.HttpContext.Session.SetString("UserName", userName);
                }

                return result;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, CrossCutting_Constants.RepositoryPolicy);
                return SignInResult.Failed;
            }
        }

        //public async Task<IdentityResult> RegisterUserAsync(RegisterViewModel model)
        //{
        //    try
        //    {
        //        var user = new ApplicationUser
        //        {
        //            Email = model.Email,
        //            UserName = model.Email,
        //            NormalizedUserName = model.Email.ToUpper(),
        //            NormalizedEmail = model.Email.ToUpper()
        //        };

        //        var result = await userManager.CreateAsync(user, model.Password);
        //        if (result.Succeeded)
        //        {
        //            if (!await roleManager.RoleExistsAsync("User"))
        //                await roleManager.CreateAsync(new IdentityRole("User"));

        //            await userManager.AddToRoleAsync(user, "User");
        //            await signInManager.SignInAsync(user, false);
        //        }
        //        return result;
        //    }
        //    catch (Exception ex)
        //    {
        //        logger.LogError(ex, CrossCutting_Constants.RepositoryPolicy);
        //        return IdentityResult.Failed();
        //    }
        //}

        public async Task LogoutAsync()
        {
            try
            {
                await signInManager.SignOutAsync();
                httpContextAccessor.HttpContext.Response.Cookies.Delete(".AspNetCore.Identity.Application");
                httpContextAccessor.HttpContext.Session.Clear();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, CrossCutting_Constants.RepositoryPolicy);
            }
        }

        public async Task<IdentityResult> ChangePasswordAsync(ResetPasswordViewModel model)
        {
            try
            {
                var user = await userManager.FindByEmailAsync(model.Email);
                if (user == null)
                    return IdentityResult.Failed(new IdentityError
                    {
                        Code = "UserNotFound",
                        Description = "User not found."
                    });

                if (user.IsPasswordResetLocked)
                    return IdentityResult.Failed(new IdentityError
                    {
                        Code = "AccountLocked",
                        Description = "Password reset is locked. Please contact to admin."
                    });

                // Validate old password
                if (!await userManager.CheckPasswordAsync(user, model.OldPassword))
                {
                    user.FailedPasswordResetAttempts += 1;
                    var attemptsLeft = Math.Max(0, MaxResetAttempts - user.FailedPasswordResetAttempts);

                    if (user.FailedPasswordResetAttempts >= MaxResetAttempts)
                    {
                        user.IsPasswordResetLocked = true;
                        await userManager.UpdateAsync(user);

                        return IdentityResult.Failed(new IdentityError
                        {
                            Code = "AccountLocked",
                            Description = "Password reset locked after 3 failed attempts."
                        });
                    }
                    await userManager.UpdateAsync(user);

                    return IdentityResult.Failed(new IdentityError
                    {
                        Code = "InvalidOldPassword",
                        Description = $"Incorrect old password. Attempts left: {attemptsLeft}"
                    });
                }

                var token = await userManager.GeneratePasswordResetTokenAsync(user);
                var result = await userManager.ResetPasswordAsync(user, token, model.NewPassword);

                if (result.Succeeded)
                {
                    user.LastPasswordChangedDate = DateTime.UtcNow;
                    user.FailedPasswordResetAttempts = 0;
                    user.IsPasswordResetLocked = false;
                    user.LockoutEnd = null;
                    await userManager.UpdateAsync(user);

                    var userPassword = await Context.TblUserPasswords.FirstOrDefaultAsync(x => x.UserId == Guid.Parse(user.Id));
                    if (userPassword != null)
                    {
                        userPassword.Password = model.NewPassword;
                        userPassword.ModifiedDateTime = DateOnly.FromDateTime(DateTime.Now);

                        await Context.SaveChangesAsync();
                    }
                    else
                    {
                        userPassword = new TblUserPassword
                        {
                            UserId = Guid.Parse(user.Id),
                            Password = model.NewPassword,
                            CreatedDateTime = DateOnly.FromDateTime(DateTime.Now),
                            ModifiedDateTime = DateOnly.FromDateTime(DateTime.Now)
                        };
                        await Context.TblUserPasswords.AddAsync(userPassword);
                        await Context.SaveChangesAsync();
                    }

                }
                return result;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, CrossCutting_Constants.RepositoryPolicy);
                return IdentityResult.Failed();
            }
        }

        public async Task<ApplicationUser> GetUserByEmailAsync(string email)
        {
            try
            {
                return await userManager.FindByEmailAsync(email);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, CrossCutting_Constants.RepositoryPolicy);
                return null;
            }
        }

        public async Task<bool> AssignRoleAsync(string email, string role)
        {
            try
            {
                var user = await userManager.FindByEmailAsync(email);
                if (user != null && !string.IsNullOrEmpty(role))
                {
                    var result = await userManager.AddToRoleAsync(user, role);
                    return result.Succeeded;
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, CrossCutting_Constants.RepositoryPolicy);
            }
            return false;
        }

        public async Task<(List<string> assignedRoles, List<string> availableRoles)> GetUserRolesAsync(string email)
        {
            try
            {
                var user = await userManager.FindByEmailAsync(email);
                var assignedRoles = await userManager.GetRolesAsync(user);
                var allRoles = await roleManager.Roles.Select(r => r.Name).ToListAsync();
                var availableRoles = allRoles.Except(assignedRoles).ToList();
                return (assignedRoles.ToList(), availableRoles);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, CrossCutting_Constants.RepositoryPolicy);
                return (new List<string>(), new List<string>());
            }
        }

        public async Task<IdentityResult> CreateUserAsync(CreateUserViewModel model)
        {
            try
            {
                var user = new ApplicationUser
                {
                    UserName = model.UserName,
                    Email = model.UserName,
                    NormalizedUserName = model.UserName.ToUpper(),
                    NormalizedEmail = model.UserName.ToUpper(),
                    PhoneNumber = model.MobileNo,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    LastPasswordChangedDate = DateTime.UtcNow,
                    FailedPasswordResetAttempts = 0,
                    IsPasswordResetLocked = false,
                    LastLoginDateTime = null
                };

                var result = await userManager.CreateAsync(user, model.UserPassword);
                if (result.Succeeded)
                {
                    IdentityRole selectedRole = null;
                    if (!string.IsNullOrEmpty(model.Role))
                    {
                        await userManager.AddToRoleAsync(user, model.Role);
                        selectedRole = await roleManager.Roles.FirstOrDefaultAsync(r => r.Name == model.Role);
                    }

                    var tblUser = new TblUserDetail
                    {
                        UserId = Guid.Parse(user.Id),
                        RoleId = Guid.Parse(selectedRole?.Id ?? Guid.Empty.ToString()),
                        UserName = model.UserName,
                        CreatedDate = DateTime.Now,
                        Role = model.Role,
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        Dob = model.Dob.HasValue ? DateOnly.FromDateTime(model.Dob.Value) : null,
                        GenderId = model.GenderId,
                        Email = model.UserName,
                        MobileNo = model.MobileNo,
                        FullAddress = model.FullAddress,
                        Status = true,
                        CustomerOrgId = model.OrganisationId,
                        BranchId = model.BranchId,
                        LastLoginDate = null,
                        NationalityId = model.NationalityId,
                        Salutation = model.Salutation
                    };

                    await Context.TblUserDetails.AddAsync(tblUser);
                    await Context.SaveChangesAsync();

                    await Context.TblUserPasswords.AddAsync(new TblUserPassword
                    {
                        UserId = Guid.Parse(user.Id),
                        Password = model.UserPassword,
                        CreatedDateTime = DateOnly.FromDateTime(DateTime.Now),
                        ModifiedDateTime = DateOnly.FromDateTime(DateTime.Now)
                    });

                    await Context.SaveChangesAsync();
                }

                return result;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, CrossCutting_Constants.RepositoryPolicy);
                return IdentityResult.Failed();
            }
        }

        public async Task<SwitchRoleViewModel> GetSwitchRolesAsync(ClaimsPrincipal user)
        {
            try
            {
                var ApplicationUser = await userManager.GetUserAsync(user);
                var roles = await userManager.GetRolesAsync(ApplicationUser);
                var activeRole = user.FindFirst("ActiveRole")?.Value;

                return new SwitchRoleViewModel
                {
                    AvailableRoles = roles.Where(r => r != activeRole).ToList(),
                    AssignedRoles = roles.ToList(),
                    CurrentActiveRole = activeRole
                };
            }
            catch (Exception ex)
            {
                logger.LogError(ex, CrossCutting_Constants.RepositoryPolicy);
                return new SwitchRoleViewModel();
            }
        }


        public async Task<bool> UpdateUserRoleClaimAsync(ClaimsPrincipal userPrincipal, string selectedRole)
        {
            try
            {
                var user = await userManager.GetUserAsync(userPrincipal);
                if (user == null)
                    return false;

                if (!await userManager.IsInRoleAsync(user, selectedRole))
                    return false;

                var existingClaims = await userManager.GetClaimsAsync(user);

                var activeClaim = existingClaims.FirstOrDefault(c => c.Type == "ActiveRole");
                if (activeClaim != null)
                    await userManager.RemoveClaimAsync(user, activeClaim);

                await userManager.AddClaimAsync(user, new Claim("ActiveRole", selectedRole));

                await signInManager.SignOutAsync();
                await signInManager.SignInAsync(user, isPersistent: false);

                return true;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, CrossCutting_Constants.RepositoryPolicy);
                return false;
            }
        }


        public async Task PopulateDropdowns(CreateUserViewModel model)
        {
            try
            {
                model.GenderList = await Context.TblMstGenders.AsNoTracking()
                    .Select(g => new SelectListItem { Value = g.GenderId.ToString(), Text = g.Gender })
                    .ToListAsync();

                model.NationalityList = await Context.TblMstNationalities.AsNoTracking()
                    .Select(n => new SelectListItem { Value = n.NationalityId.ToString(), Text = n.Nationality })
                    .ToListAsync();

                model.OrganisationList = await Context.TblMstCustomerOrgs.AsNoTracking()
                    .Select(x => new SelectListItem { Value = x.CustomerOrgId.ToString(), Text = x.CustomerOrgName })
                    .ToListAsync();

            }
            catch (Exception ex)
            {
                logger.LogError(ex, CrossCutting_Constants.RepositoryPolicy);
            }
        }
        public async Task<List<SelectListItem>> GetBranchesByOrganisation(long organisationId)
        {
            try
            {
                return await Context.TblMstBranches.AsNoTracking()
                    .Where(b => b.CustomerOrgId == organisationId)
                    .Select(b => new SelectListItem { Value = b.BranchId.ToString(), Text = b.BranchName })
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, CrossCutting_Constants.RepositoryPolicy);
                return new List<SelectListItem>();
            }
        }
        public async Task<List<SelectListItem>> GetRolesByOrganisation(long organisationId)
        {
            try
            {
                var org = await Context.TblMstCustomerOrgs.FirstOrDefaultAsync(x => x.CustomerOrgId == organisationId);
                if (org != null && org.CustomerOrgName == "LetsFloruit")
                {
                    return await roleManager.Roles.AsNoTracking()
                    .Select(r => new SelectListItem { Value = r.Name, Text = r.Name })
                    .ToListAsync();
                }
                else
                {
                    return await roleManager.Roles.AsNoTracking()
                .Where(r => r.Name == "Agent")
                .Select(r => new SelectListItem { Value = r.Name, Text = r.Name })
                .ToListAsync();

                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, CrossCutting_Constants.RepositoryPolicy);
                return new List<SelectListItem>();
            }
        }
        public async Task<List<SelectListItem>> GetAssignUnderUsersByOrganisation(long organisationId)
        {
            try
            {
                var org = await Context.TblMstCustomerOrgs.AsNoTracking().FirstOrDefaultAsync(x => x.CustomerOrgId == organisationId);

                if (org != null && org.CustomerOrgName == "LetsFloruit")
                {
                    var allUsers = await userManager.Users.AsNoTracking()
                        .Select(u => new SelectListItem
                        {
                            Value = u.Id,
                            Text = u.UserName
                        }).ToListAsync();

                    return allUsers;
                }
                else
                {
                    var agentUsers = await userManager.GetUsersInRoleAsync("Agent");

                    return agentUsers
                        .Select(u => new SelectListItem
                        {
                            Value = u.Id,
                            Text = u.UserName
                        }).ToList();
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, CrossCutting_Constants.RepositoryPolicy);
                return new List<SelectListItem>();
            }
        }

        public async Task<bool> IsLockedOutAsync(ApplicationUser user)
        {
            try
            {
                return await userManager.IsLockedOutAsync(user);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, CrossCutting_Constants.RepositoryPolicy);
                return false;
            }
        }

        public async Task AccessFailedAsync(ApplicationUser user)
        {
            try
            {
                await userManager.AccessFailedAsync(user);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, CrossCutting_Constants.RepositoryPolicy);
            }
        }

        public async Task ResetAccessFailedCountAsync(ApplicationUser user)
        {
            try
            {
                await userManager.ResetAccessFailedCountAsync(user);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, CrossCutting_Constants.RepositoryPolicy);
            }
        }

        public async Task<IEnumerable<UserListViewModel>> GetUsersAsync()
        {
            try
            {
                IQueryable<ApplicationUser> usersQuery = userManager.Users.AsNoTracking();

                var query =
                    from u in usersQuery
                    join ud in Context.TblUserDetails
                            on u.Id equals ud.UserId.ToString() into gUd
                    from ud in gUd.DefaultIfEmpty()

                    join b in Context.TblMstBranches
                            on ud.BranchId equals b.BranchId into gB
                    from b in gB.DefaultIfEmpty()

                    join co in Context.TblMstCustomerOrgs
                            on ud.CustomerOrgId equals co.CustomerOrgId into gCo
                    from co in gCo.DefaultIfEmpty()

                    select new { u, ud, b, co };

                var raw = await query.ToListAsync();
                var list = new List<UserListViewModel>();

                foreach (var item in raw)
                {
                    var roles = await userManager.GetRolesAsync(item.u);

                    list.Add(new UserListViewModel
                    {
                        UserId = Guid.Parse(item.u.Id),
                        Name = item.ud != null ? $"{item.ud.FirstName} {item.ud.LastName}".Trim() : string.Empty,
                        Email = item.u.Email,
                        Mobile = item.ud?.MobileNo ?? "N/A",
                        CreatedDate = item.ud?.CreatedDate ?? null,
                        Branch = item.b?.BranchName ?? "N/A",
                        Organisation = item.co?.CustomerOrgName ?? "N/A",
                        Roles = roles,
                        Role = string.Join(", ", roles),
                        IsActive = item.ud?.Status ?? true,
                        IsUserLockOut = await userManager.IsLockedOutAsync(item.u)
                    });
                }

                return list;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, CrossCutting_Constants.RepositoryPolicy);
                return Enumerable.Empty<UserListViewModel>();
            }
        }
        public async Task<bool> UpdateUserStatusAsync(Guid userId, bool isActive)
        {
            try
            {
                var user = await Context.TblUserDetails.FirstOrDefaultAsync(x => x.UserId == userId);
                if (user == null) return false;
                user.Status = isActive;
                await Context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, CrossCutting_Constants.RepositoryPolicy);
                return false;
            }
        }
        public async Task<bool> UpdateUserLockoutStatusAsync(Guid userId, bool isLockedOut)
        {
            var user = await userManager.FindByIdAsync(userId.ToString());
            if (user == null) return false;

            if (isLockedOut)
            {
                // lock user for 100 years
                await userManager.SetLockoutEndDateAsync(user, DateTimeOffset.MaxValue);
            }
            else
            {
                // unlock user
                await userManager.SetLockoutEndDateAsync(user, null);
                await userManager.ResetAccessFailedCountAsync(user);

                user.FailedPasswordResetAttempts = 0;
                user.IsPasswordResetLocked = false;

                await userManager.UpdateAsync(user);
            }

            return true;
        }
        public async Task<bool> IsUserInActive(string userName)
        {
            try
            {
                var user = await Context.TblUserDetails.AsNoTracking().FirstOrDefaultAsync(x => x.UserName == userName);
                if (user != null && user.Status == false)
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, CrossCutting_Constants.RepositoryPolicy);
                return false;
            }
        }
        public async Task<TblUserDetail> GetUserDetailsByUserName(string userName)
        {
            try
            {
                return await Context.TblUserDetails.AsNoTracking().FirstOrDefaultAsync(x => x.UserName == userName);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, CrossCutting_Constants.RepositoryPolicy);
                return null;
            }
        }
    }
}

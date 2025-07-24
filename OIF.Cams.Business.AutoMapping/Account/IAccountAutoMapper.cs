using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using OIF.Cams.Data.DAC;
using OIF.Cams.Data.DAC.AppDbContext;
using OIF.Cams.Model.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace OIF.Cams.Business.AutoMapping.Account
{
    public interface IAccountAutoMapper
    {
        Task<bool> CheckPasswordAsync(ApplicationUser user, string password);
        //Task<IdentityResult> RegisterUserAsync(RegisterViewModel model);
        Task LogoutAsync();
        Task<IdentityResult> ChangePasswordAsync(ResetPasswordViewModel model);
        Task<ApplicationUser> GetUserByEmailAsync(string email);
        Task<bool> AssignRoleAsync(string email, string role);
        Task<(List<string> assignedRoles, List<string> availableRoles)> GetUserRolesAsync(string email);
        Task<IdentityResult> CreateUserAsync(CreateUserViewModel model);
        Task<SwitchRoleViewModel> GetSwitchRolesAsync(ClaimsPrincipal user);
        Task<bool> UpdateUserRoleClaimAsync(ClaimsPrincipal user, string selectedRole);
        Task PopulateDropdowns(CreateUserViewModel model);
        Task<List<SelectListItem>> GetBranchesByOrganisation(long organisationId);
        Task<bool> IsLockedOutAsync(ApplicationUser user);
        Task SignInAsync(ApplicationUser user, bool rememberMe);
        Task AccessFailedAsync(ApplicationUser user);
        Task ResetAccessFailedCountAsync(ApplicationUser user);
        Task<SignInResult> PasswordSignInAsync(string userName, string password, bool rememberMe);
        Task<IEnumerable<UserListViewModel>> GetUsersAsync();
        Task<bool> UpdateUserStatusAsync(Guid userId, bool isActive);
        Task<bool> UpdateUserLockoutStatusAsync(Guid userId, bool isLockedOut);
        Task<bool> IsUserInActive(string userName);
        Task<TblUserDetail> GetUserDetailsByUserName(string userName);
        Task<List<SelectListItem>> GetRolesByOrganisation(long organisationId);
        Task<List<SelectListItem>> GetAssignUnderUsersByOrganisation(long organisationId);
    }
}

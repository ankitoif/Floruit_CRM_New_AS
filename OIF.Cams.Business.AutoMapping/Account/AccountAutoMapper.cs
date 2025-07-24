using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using OIF.Cams.CrossCutting;
using OIF.Cams.Data.DAC;
using OIF.Cams.Data.DAC.AppDbContext;
using OIF.Cams.Data.Repository.Account;
using OIF.Cams.Model.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace OIF.Cams.Business.AutoMapping.Account
{
    public class AccountAutoMapper : IAccountAutoMapper
    {
        private readonly ILogger<AccountAutoMapper> logger;
        private readonly IAccountRepo iAccountRepo;
        public AccountAutoMapper(ILogger<AccountAutoMapper> _logger, IAccountRepo _iAccountRepo)
        {
            logger = _logger;
            iAccountRepo = _iAccountRepo;
        }

        public async Task<bool> AssignRoleAsync(string email, string role)
        {
            try
            {
                return await iAccountRepo.AssignRoleAsync(email, role);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, CrossCutting_Constants.AMPolicy);
                return false;
            }
        }

        public async Task<IdentityResult> ChangePasswordAsync(ResetPasswordViewModel model)
        {
            try
            {
                return await iAccountRepo.ChangePasswordAsync(model);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, CrossCutting_Constants.AMPolicy);
                return IdentityResult.Failed(new IdentityError { Description = "An error occurred while changing the password." });
            }
        }

        public async Task<IdentityResult> CreateUserAsync(CreateUserViewModel model)
        {
            try
            {
                return await iAccountRepo.CreateUserAsync(model);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, CrossCutting_Constants.AMPolicy);
                return IdentityResult.Failed(new IdentityError { Description = "An error occurred while creating the user." });
            }
        }

        public async Task<SwitchRoleViewModel> GetSwitchRolesAsync(ClaimsPrincipal user)
        {
            try
            {
                return await iAccountRepo.GetSwitchRolesAsync(user);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, CrossCutting_Constants.AMPolicy);
                return new SwitchRoleViewModel
                {
                    AssignedRoles = new List<string>(),
                    AvailableRoles = new List<string>()
                };
            }
        }

        public async Task<ApplicationUser> GetUserByEmailAsync(string email)
        {
            try
            {
                return await iAccountRepo.GetUserByEmailAsync(email);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, CrossCutting_Constants.AMPolicy);
                return null;
            }
        }

        public async Task<(List<string> assignedRoles, List<string> availableRoles)> GetUserRolesAsync(string email)
        {
            try
            {
                return await iAccountRepo.GetUserRolesAsync(email);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, CrossCutting_Constants.AMPolicy);
                return (new List<string>(), new List<string>());
            }
        }

        public async Task LogoutAsync()
        {
            try
            {
                await iAccountRepo.LogoutAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, CrossCutting_Constants.AMPolicy);
            }
        }

        public async Task<bool> CheckPasswordAsync(ApplicationUser user, string password)
        {
            try
            {
                return await iAccountRepo.CheckPasswordAsync(user, password);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, CrossCutting_Constants.AMPolicy);
                return false;
            }
        }
        public async Task SignInAsync(ApplicationUser user, bool rememberMe)
        {
            try
            {
                await iAccountRepo.SignInAsync(user, rememberMe);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, CrossCutting_Constants.AMPolicy);
            }
        }

        public async Task PopulateDropdowns(CreateUserViewModel model)
        {
            try
            {
                await iAccountRepo.PopulateDropdowns(model);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, CrossCutting_Constants.AMPolicy);
            }
        }

        //public async Task<IdentityResult> RegisterUserAsync(RegisterViewModel model)
        //{
        //    try
        //    {
        //        return await iAccountRepo.RegisterUserAsync(model);
        //    }
        //    catch (Exception ex)
        //    {
        //        logger.LogError(ex, CrossCutting_Constants.AMPolicy);
        //        return IdentityResult.Failed(new IdentityError { Description = "An error occurred while registering the user." });
        //    }
        //}

        public async Task<bool> UpdateUserRoleClaimAsync(ClaimsPrincipal user, string selectedRole)
        {
            try
            {
                return await iAccountRepo.UpdateUserRoleClaimAsync(user, selectedRole);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, CrossCutting_Constants.AMPolicy);
                return false;
            }
        }
        public async Task<List<SelectListItem>> GetBranchesByOrganisation(long organisationId)
        {
            try
            {
                return await iAccountRepo.GetBranchesByOrganisation(organisationId);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, CrossCutting_Constants.AMPolicy);
                return new List<SelectListItem>();
            }
        }
        public async Task<List<SelectListItem>> GetRolesByOrganisation(long organisationId)
        {
            try
            {
                return await iAccountRepo.GetRolesByOrganisation(organisationId);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, CrossCutting_Constants.AMPolicy);
                return new List<SelectListItem>();
            }
        }
        public async Task<List<SelectListItem>> GetAssignUnderUsersByOrganisation(long organisationId)
        {
            try
            {
                return await iAccountRepo.GetAssignUnderUsersByOrganisation(organisationId);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, CrossCutting_Constants.AMPolicy);
                return new List<SelectListItem>();
            }
        }
        public async Task<bool> IsLockedOutAsync(ApplicationUser user)
        {
            try
            {
                return await iAccountRepo.IsLockedOutAsync(user);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, CrossCutting_Constants.AMPolicy);
                return false;
            }
        }

        public async Task AccessFailedAsync(ApplicationUser user)
        {
            try
            {
                await iAccountRepo.AccessFailedAsync(user);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, CrossCutting_Constants.AMPolicy);
            }
        }

        public async Task ResetAccessFailedCountAsync(ApplicationUser user)
        {
            try
            {
                await iAccountRepo.ResetAccessFailedCountAsync(user);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, CrossCutting_Constants.AMPolicy);
            }
        }
        public async Task<SignInResult> PasswordSignInAsync(string userName, string password, bool rememberMe)
        {
            try
            {
                return await iAccountRepo.PasswordSignInAsync(userName, password, rememberMe);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, CrossCutting_Constants.RepositoryPolicy);
                return SignInResult.Failed;
            }
        }

        public async Task<IEnumerable<UserListViewModel>> GetUsersAsync()
        {
            try
            {
                return await iAccountRepo.GetUsersAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, CrossCutting_Constants.RepositoryPolicy);
                return new List<UserListViewModel>();
            }
        }
        public async Task<bool> UpdateUserStatusAsync(Guid userId, bool isActive)
        {
            try
            {
                return await iAccountRepo.UpdateUserStatusAsync(userId, isActive);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, CrossCutting_Constants.AMPolicy);
                return false;
            }
        }
        public async Task<bool> UpdateUserLockoutStatusAsync(Guid userId, bool isLockedOut)
        {
            try
            {
                return await iAccountRepo.UpdateUserLockoutStatusAsync(userId, isLockedOut);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, CrossCutting_Constants.AMPolicy);
                return false;
            }
        }
        public async Task<bool> IsUserInActive(string userName)
        {
            try
            {
                return await iAccountRepo.IsUserInActive(userName);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, CrossCutting_Constants.AMPolicy);
                return false;
            }
        }
        public async Task<TblUserDetail> GetUserDetailsByUserName(string userName)
        {
            try
            {
                return await iAccountRepo.GetUserDetailsByUserName(userName);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, CrossCutting_Constants.AMPolicy);
                return null;
            }
        }
    }
}

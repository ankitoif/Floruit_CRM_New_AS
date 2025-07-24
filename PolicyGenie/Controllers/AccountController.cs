using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OIF.Cams.Business.AutoMapping.Account;
using OIF.Cams.CrossCutting;
using OIF.Cams.Model.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace PolicyGenie.Controllers
{
    public class AccountController : Controller
    {
        private readonly ILogger<AccountController> logger;
        private readonly IAccountAutoMapper iAccountAutoMapper;

        public AccountController(IAccountAutoMapper _iAccountAutoMapper, ILogger<AccountController> _logger)
        {
            iAccountAutoMapper = _iAccountAutoMapper;
            logger = _logger;
        }

        [HttpGet]
        public IActionResult Login()
        {
            var userName = HttpContext.Session.GetString("UserName");
            var activeRole = HttpContext.Session.GetString("ActiveRole");
            if (User.Identity.IsAuthenticated && (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(activeRole)))
            {
                return RedirectToAction("SilentLogout", "Account");
            }
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                if (activeRole == "Agent")
                {
                    return RedirectToAction("ServiceRequestDashboard", "ServiceRequest", new { dashboardType = "TotalTask" });
                }
                else
                {
                    return RedirectToAction("ServiceRequestDashboard", "ServiceRequest", new { dashboardType = "Submitted" });
                }
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    TempData["Error"] = "Please enter UserName and Password!";
                    return View(model);
                }

                var user = await iAccountAutoMapper.GetUserByEmailAsync(model.Email);

                if (user == null)
                {
                    ModelState.AddModelError(nameof(model.Email), "Please enter valid UserName to continue.");
                    return View(model);
                }

                var result = await iAccountAutoMapper.PasswordSignInAsync(user.UserName, model.Password, model.RememberMe);

                if (HttpContext.Session.GetString("PasswordExpired") == "true")
                {
                    HttpContext.Session.Remove("PasswordExpired");
                    await iAccountAutoMapper.LogoutAsync();
                    TempData["Error"] = "Your password has expired. Please reset it to continue.";
                    return RedirectToAction("ResetPassword", new { userName = model.Email });
                }

                if (result.Succeeded)
                {
                    TempData["Success"] = "Login successful!";
                    return RedirectToAction("Index", "Home");
                }

                if (result.IsLockedOut)
                {
                    return RedirectToAction("AccountLocked");
                }

                if (await iAccountAutoMapper.IsUserInActive(model.Email))
                {
                    return RedirectToAction("InActiveUserAccount");
                }

                if (result.IsNotAllowed)
                {
                    ModelState.AddModelError(string.Empty, "You are not allowed to log in at this time.");
                    return View(model);
                }

                ModelState.AddModelError(nameof(model.Password), "Invalid password. Please try again.");
                return View(model);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, CrossCutting_Constants.ControllerPolicy);
                return View("Error");
            }
        }

        [HttpGet]
        public IActionResult AccountLocked()
        {
            return View("_AccountLocked");
        }
        [HttpGet]
        public IActionResult InActiveUserAccount()
        {
            return View("_InActiveUserAccount");
        }
        public IActionResult Register()
        {
            return View();
        }

        public IActionResult VerifyEmail()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> VerifyEmail(VerifyEmailViewModel model)
        {
            try
            {
                await iAccountAutoMapper.LogoutAsync();
                if (ModelState.IsValid)
                {
                    var user = await iAccountAutoMapper.GetUserByEmailAsync(model.Email);
                    if (user == null)
                    {
                        ModelState.AddModelError(string.Empty, "Email does not exist.");
                        return View(model);
                    }
                    TempData["Success"] = "Email Verified successfully!";
                    return RedirectToAction("ResetPassword", "Account", new { userName = user.UserName });
                }
                return View(model);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, CrossCutting_Constants.ControllerPolicy);
                return View("Error");
            }
        }

        [HttpGet]
        public async Task<IActionResult> ResetPassword(string userName)
        {
            try
            {
                await iAccountAutoMapper.LogoutAsync();

                if (string.IsNullOrEmpty(userName))
                    return RedirectToAction("VerifyEmail", "Account");

                var user = await iAccountAutoMapper.GetUserByEmailAsync(userName);
                if (user == null)
                    return RedirectToAction("VerifyEmail", "Account");

                // ✅ Check lockout status before showing the page
                if (await iAccountAutoMapper.IsLockedOutAsync(user))
                    return RedirectToAction("AccountLocked");

                // ✅ Check Active status before showing the page
                if (await iAccountAutoMapper.IsUserInActive(userName))
                {
                    return RedirectToAction("InActiveUserAccount");
                }

                return View(new ResetPasswordViewModel { Email = userName });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, CrossCutting_Constants.ControllerPolicy);
                return View("Error");
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    ModelState.AddModelError(string.Empty, "Something went wrong! Try again.");
                    return View(model);
                }

                var result = await iAccountAutoMapper.ChangePasswordAsync(model);

                if (result.Succeeded)
                {
                    await iAccountAutoMapper.LogoutAsync();
                    TempData["Success"] = "Your password has been reset successfully.";
                    return RedirectToAction("Login", "Account");
                }

                if (result.Errors.Any(e => e.Code == "AccountLocked")) 
                {
                    return RedirectToAction("AccountLocked");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                return View(model);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, CrossCutting_Constants.ControllerPolicy);
                return View("Error");
            }
        }
        [HttpGet]
        public IActionResult SilentLogout()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            try
            {
                await iAccountAutoMapper.LogoutAsync();
                return RedirectToAction("Login", "Account");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, CrossCutting_Constants.ControllerPolicy);
                return View("Error");
            }
        }

        public async Task<IActionResult> SwitchRole()
        {
            try
            {
                var obj = await iAccountAutoMapper.GetSwitchRolesAsync(User);
                return PartialView("_SwitchRolePartial", obj);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, CrossCutting_Constants.ControllerPolicy);
                return BadRequest();
            }
        }

        [HttpPost]
        public async Task<IActionResult> SwitchRole(string selectedRole)
        {
            try
            {
                var success = await iAccountAutoMapper.UpdateUserRoleClaimAsync(User, selectedRole);
                if (!success)
                    return BadRequest("Invalid role selection");

                HttpContext.Session.SetString("ActiveRole", selectedRole);

                return Ok(new { redirectUrl = Url.Action("Index", "Home") });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, CrossCutting_Constants.ControllerPolicy);
                return BadRequest("An error occurred.");
            }
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateUser()
        {
            try
            {
                var model = new CreateUserViewModel();
                await iAccountAutoMapper.PopulateDropdowns(model);
                return View(model);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, CrossCutting_Constants.ControllerPolicy);
                return View("Error");
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateUser(CreateUserViewModel model)
        {
            try
            {
                if (await iAccountAutoMapper.GetUserByEmailAsync(model.UserName) != null)
                {
                    ModelState.AddModelError("UserName", "Email already exists.");
                }
                if (!ModelState.IsValid)
                {
                    await iAccountAutoMapper.PopulateDropdowns(model);
                    return View(model);
                }

                var result = await iAccountAutoMapper.CreateUserAsync(model);
                if (result.Succeeded)
                {
                    TempData["Success"] = "User created successfully!";
                    return RedirectToAction("GetAllUserList");
                }

                foreach (var error in result.Errors)
                    ModelState.AddModelError("", error.Description);

                await iAccountAutoMapper.PopulateDropdowns(model);
                return RedirectToAction("ServiceRequestDashboard", "ServiceRequest", new { dashboardType = "Submitted" });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, CrossCutting_Constants.ControllerPolicy);
                return View("Error");
            }
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult AssignRole()
        {
            try
            {
                return View(new AssignRoleViewModel());
            }
            catch (Exception ex)
            {
                logger.LogError(ex, CrossCutting_Constants.ControllerPolicy);
                return View("Error");
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> SearchUserRoles(AssignRoleViewModel model)
        {
            try
            {
                var user = await iAccountAutoMapper.GetUserByEmailAsync(model.Email);
                if (user == null)
                {
                    ModelState.AddModelError("Email", "User not found.");
                    return View("AssignRole", model);
                }

                var (assignedRoles, availableRoles) = await iAccountAutoMapper.GetUserRolesAsync(model.Email);
                model.AssignedRoles = assignedRoles;
                model.AvailableRoles = availableRoles.Select(r => new SelectListItem { Text = r, Value = r }).ToList();

                return View("AssignRole", model);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, CrossCutting_Constants.ControllerPolicy);
                return View("Error");
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> SubmitRole(AssignRoleViewModel model)
        {
            try
            {
                var success = await iAccountAutoMapper.AssignRoleAsync(model.Email, model.SelectedRole);
                if (!success)
                {
                    ModelState.AddModelError("Email", "User not found or role is invalid.");
                    return View("AssignRole", model);
                }

                TempData["Success"] = "Role assigned successfully.";
                return RedirectToAction("AssignRole");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, CrossCutting_Constants.ControllerPolicy);
                return View("Error");
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetOrganisationMappings(long organisationId)
        {
            try
            {
                var branches = await iAccountAutoMapper.GetBranchesByOrganisation(organisationId);
                var roles = await iAccountAutoMapper.GetRolesByOrganisation(organisationId);

                return Json(new
                {
                    branches,
                    roles
                });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, CrossCutting_Constants.ControllerPolicy);
                return Json(new { branches = new List<SelectListItem>(), roles = new List<SelectListItem>() });
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetAssignUnderUsers(long organisationId)
        {
            try
            {
                var assignUnderUsers = await iAccountAutoMapper.GetAssignUnderUsersByOrganisation(organisationId);

                if (assignUnderUsers != null && assignUnderUsers.Any())
                {
                    return Json(assignUnderUsers);
                }
                return Json(new List<SelectListItem>());
            }
            catch (Exception ex)
            {
                logger.LogError(ex, CrossCutting_Constants.ControllerPolicy);
                return Json(new List<SelectListItem>());
            }
        }

        public async Task<IActionResult> GetAllUserList()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> GetAllUserLists()
        {
            IEnumerable<UserListViewModel> users = await iAccountAutoMapper.GetUsersAsync();
            return PartialView("_UserListPartial", users);
        }
        [HttpPost]
        public async Task<IActionResult> ChangeUserStatus(Guid userId, bool isActive)
        {
            var result = await iAccountAutoMapper.UpdateUserStatusAsync(userId, isActive);
            if (result)
                return Ok();
            return BadRequest();
        }
        [HttpPost]
        public async Task<IActionResult> ChangeLockoutStatus(Guid userId, bool isLockedOut)
        {
            var result = await iAccountAutoMapper.UpdateUserLockoutStatusAsync(userId, isLockedOut);
            if (result)
                return Ok();
            return BadRequest();
        }
    }
}

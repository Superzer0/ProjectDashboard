using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http;
using Common.Logging;
using Dashboard.Infrastructure.Controllers;
using Dashboard.Models.Account;
using Dashboard.UI.Objects.Auth;
using Dashboard.UI.Objects.Providers;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Dashboard.Controllers.API
{
    [Authorize]
    [RoutePrefix("api/account")]
    public class AccountController : BaseController
    {
        private readonly IConfigureDashboard _configureDashboard;
        private readonly ILog _log = LogManager.GetLogger<AccountController>();

        public AccountController(IConfigureDashboard configureDashboard)
        {
            _configureDashboard = configureDashboard;
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("register", Name = "RegisterUser")]
        public async Task<IHttpActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var user = new DashboardUser { UserName = model.UserName, Email = model.Email };
            var userCreatedResult = await UserManager.CreateAsync(user, model.Password);

            // if some errors encountered - return them to the user
            var userCreatedValidationResults = ParseErrorResult(userCreatedResult);
            if (userCreatedValidationResults != null) return userCreatedValidationResults;

            // add standard User role to user
            var roleAddedResult = await UserManager.AddToRoleAsync(user.Id, DashboardRoles.User);
            var roleAddedValidationResult = ParseErrorResult(roleAddedResult);
            if (roleAddedValidationResult != null)
            {
                // adding role failed, log errors, return OK to user (internal error)
                _log.Error(l => l($"[Internal Error]: User {user.UserName} created but standard {DashboardRoles.User} role not added" +
                                  $" . Errors: {GetModelStateSummary()}"));
            }

            _log.Info(p => p($"Created user: {user.UserName} with email: {user.Email}"));
            return Ok();
        }

        [HttpPost]
        [Route("logoff", Name = "LogOffUser")]
        public IHttpActionResult LogOff()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return Ok();
        }

        [HttpPost]
        [Route("changePass", Name = "ChangePass")]
        public async Task<IHttpActionResult> ChangePassword(ChangePasswordViewModel passwordViewModel)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var user = await GetCurrentUser();

            var actionResult =
                await
                    UserManager.ChangePasswordAsync(user.Id, passwordViewModel.CurrentPassword,
                        passwordViewModel.NewPassword);
            var validationResult = ParseErrorResult(actionResult);
            if (validationResult != null) return validationResult;

            _log.Info($"changed password for {user.Id}");
            return Ok();
        }

        [HttpGet]
        [Route("user-info", Name = "UserInfo")]
        [Authorize]
        public IHttpActionResult GetUserInfo()
        {
            var identity = User.Identity as ClaimsIdentity;
            if (identity == null) return BadRequest();

            return Ok(new UserInfoViewModel
            {
                Id = identity.Claims.FirstOrDefault(p => p.Type == ClaimTypes.NameIdentifier)?.Value,
                UserName = identity.Claims.First(p => p.Type == ClaimTypes.Name)?.Value,
                Email = identity.Claims.FirstOrDefault(p => p.Type == ClaimTypes.Email)?.Value,
                Roles = identity.Claims.Where(p => p.Type == ClaimTypes.Role).Select(d => d.Value)
            });
        }

        [HttpGet]
        [Route("allusers")]
        [Authorize(Roles = DashboardRoles.Admin)]
        public IHttpActionResult GetAllUsers()
        {
            var roles = RoleManager.Roles.ToList().Select(p => new IdentityRole { Id = p.Id, Name = p.Name });

            var users = UserManager.Users.ToList().Select(p => new UserInfoViewModel
            {
                Email = p.Email,
                Id = p.Id,
                UserName = p.UserName,
                Roles = p.Roles
                    .Select(r => roles.FirstOrDefault(w => w.Id.Equals(r.RoleId))?.Name)
                    .Where(r => !string.IsNullOrEmpty(r)).ToList()
            }).OrderBy(p => p.UserName).ToList();

            return Ok(users);
        }

        [HttpDelete]
        [Route("delete/{userId:guid}")]
        [Authorize(Roles = DashboardRoles.Admin)]
        public async Task<IHttpActionResult> DeleteUser(string userId)
        {
            if (string.IsNullOrEmpty(userId)) return BadRequest();

            var dashboardUser = await UserManager.FindByIdAsync(userId);
            if (dashboardUser == null) return NotFound();

            var actionResult = await UserManager.DeleteAsync(dashboardUser);
            var validationResult = ParseErrorResult(actionResult);

            if (validationResult != null) return validationResult;

            _log.Info(m => m($"User {userId} deleted"));
            return Ok();
        }

        [HttpPost]
        [Route("change-roles/{userId:guid}")]
        [Authorize(Roles = DashboardRoles.Admin)]
        public async Task<IHttpActionResult> ChangeRoles([FromUri] string userId, [FromBody] ChangeRolesViewModel viewModel)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var dashboardUser = await UserManager.FindByIdAsync(userId);
            if (dashboardUser == null) return NotFound();

            var allRoles = RoleManager.Roles.ToList();
            ValidateRoles(allRoles, viewModel.RolesToRemove);
            ValidateRoles(allRoles, viewModel.RolesToAdd);

            if (!ModelState.IsValid) return BadRequest(ModelState); // some invalid roles in request

            if (viewModel.RolesToAdd.Any())
            {
                foreach (var role in viewModel.RolesToAdd)
                {
                    if (await UserManager.IsInRoleAsync(dashboardUser.Id, role)) continue;

                    var additionResult = await UserManager.AddToRoleAsync(dashboardUser.Id, role);
                    ParseErrorResult(additionResult);
                }
            }

            if (viewModel.RolesToRemove.Any())
            {
                foreach (var role in viewModel.RolesToRemove)
                {
                    if (!await UserManager.IsInRoleAsync(dashboardUser.Id, role)) continue;

                    var removeResult = await UserManager.RemoveFromRoleAsync(dashboardUser.Id, role);
                    ParseErrorResult(removeResult);
                }
            }

            if (!ModelState.IsValid) return BadRequest(ModelState); // some invalid roles in request

            return Ok();
        }

        [HttpGet]
        [Route("admin-party/state")]
        [Authorize(Roles = DashboardRoles.Admin)]
        [AllowAnonymous]
        public bool AdminPartyState()
        {
            return _configureDashboard.GetAdminPartyState();
        }

        [HttpPost]
        [Route("admin-party/change")]
        [Authorize(Roles = DashboardRoles.Admin)]
        public async Task<IHttpActionResult> SetAdminPartyState([FromBody] bool changeStatus)
        {
            var adminPartyStatus = _configureDashboard.GetAdminPartyState();
            if (changeStatus == adminPartyStatus) return Ok();

            if (changeStatus)
            {
                var dashboardUser = await GetCurrentUser();
                if (dashboardUser == null) return NotFound();

                if (!await UserManager.IsInRoleAsync(dashboardUser.Id, DashboardRoles.Admin))
                {
                    var identityResult = await UserManager.AddToRoleAsync(dashboardUser.Id, DashboardRoles.Admin);
                    var actionResult = ParseErrorResult(identityResult);
                    if (actionResult != null) return actionResult;
                }
            }

            _configureDashboard.SetAdminPartyState(changeStatus);

            return Ok();
        }

        private void ValidateRoles(List<IdentityRole> allRoles, IEnumerable<string> roles)
        {
            foreach (var role in roles)
            {
                if (string.IsNullOrEmpty(role) || !allRoles.Any(p => p.Name.Equals(role)))
                {
                    ModelState.AddModelError(string.Empty, $"role {role} does not exist");
                }
            }
        }

        private IHttpActionResult ParseErrorResult(IdentityResult result)
        {
            if (result.Succeeded) return null; // everything ok

            if (result.Errors != null)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(Guid.NewGuid().ToString(), error);
                }
            }

            if (ModelState.IsValid) // no model state errors
            {
                return BadRequest();
            }

            return BadRequest(ModelState);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http;
using Common.Logging;
using Dashboard.Infrastructure.Controllers;
using Dashboard.Infrastructure.Identity;
using Dashboard.Models.Account;
using Dashboard.UI.Objects.Auth;
using Dashboard.UI.Objects.Services;
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
            var result = await UserManager.CreateAsync(user, model.Password);

            var validationResult = ParseErrorResult(result);
            if (validationResult != null) return validationResult;

            await UserManager.AddToRoleAsync(user.Id, DashboardRoles.User);
            _log.Info(p => p("Registered User " + user.UserName));
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

            var user = await UserManager.FindByNameAsync(User.Identity.Name);

            var actionResult =
                await
                    UserManager.ChangePasswordAsync(user.Id, passwordViewModel.CurrentPassword,
                        passwordViewModel.NewPassword);
            var validationResult = ParseErrorResult(actionResult);

            return validationResult ?? Ok();
        }

        [HttpGet]
        [Route("user-info", Name = "UserInfo")]
        [Authorize]
        public IHttpActionResult GetUserInfo()
        {
            var identity = User.Identity as ClaimsIdentity;
            return Ok(new
            {
                id = identity?.Claims.FirstOrDefault(p => p.Type == ClaimTypes.NameIdentifier)?.Value,
                userName = identity?.Claims.First(p => p.Type == ClaimTypes.Name)?.Value,
                email = identity?.Claims.FirstOrDefault(p => p.Type == ClaimTypes.Email)?.Value,
                roles = identity?.Claims.Where(p => p.Type == ClaimTypes.Role).Select(d => d?.Value)
            });
        }

        [HttpGet]
        [Route("allusers")]
        [Authorize(Roles = DashboardRoles.Admin)]
        public IHttpActionResult GetAllUsers()
        {
            var roles = RoleManager.Roles.ToList().Select(p => new IdentityRole { Id = p.Id, Name = p.Name });
            var users = UserManager.Users.ToList().Select(p => new
            {
                p.Email,
                p.Id,
                p.UserName,
                roles = p.Roles.Select(r => roles.FirstOrDefault(w => w.Id.Equals(r.RoleId))?.Name)
            }).ToList();

            return Ok(users);
        }

        [HttpDelete]
        [Route("delete/{userId:guid}")]
        [Authorize(Roles = DashboardRoles.Admin)]
        public async Task<IHttpActionResult> DeleteUser(string userId)
        {
            var dashboardUser = await UserManager.FindByIdAsync(userId);
            if (dashboardUser == null) return NotFound();

            var actionResult = await UserManager.DeleteAsync(dashboardUser);
            var validationResult = ParseErrorResult(actionResult);

            return validationResult ?? Ok();
        }

        [HttpPost]
        [Route("change-roles/{userId:guid}")]
        [Authorize(Roles = DashboardRoles.Admin)]
        public async Task<IHttpActionResult> ChangeRoles([FromUri] string userId, [FromBody] ChangeRolesViewModel viewModel)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var dashboardUser = await UserManager.FindByIdAsync(userId);
            if (dashboardUser == null) return NotFound();

            await CheckIfRolesAreValid(viewModel.RolesToRemove);
            await CheckIfRolesAreValid(viewModel.RolesToAdd);

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

            if (!ModelState.IsValid) return BadRequest(ModelState); // some invalid roles in request

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
        //[Authorize(Roles = DashboardRoles.Admin)]
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
                var dashboardUser = await UserManager.FindByNameAsync(User.Identity.Name);

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

        private async Task CheckIfRolesAreValid(IEnumerable<string> roles)
        {
            var verifiedRoles = new List<IdentityRole>();

            foreach (var role in roles)
            {
                var verifiedRole = await RoleManager.FindByNameAsync(role);
                if (verifiedRole != null)
                {
                    verifiedRoles.Add(verifiedRole);
                }
                else
                {
                    ModelState.AddModelError(string.Empty, $"role {role} does not exist");
                }
            }
        }

        private IHttpActionResult ParseErrorResult(IdentityResult result)
        {
            if (result == null) return InternalServerError();

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

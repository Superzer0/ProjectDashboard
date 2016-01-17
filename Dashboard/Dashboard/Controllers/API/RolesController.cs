using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using Dashboard.Infrastructure.Controllers;
using Dashboard.Infrastructure.Identity;
using Dashboard.Models.Account;
using Microsoft.AspNet.Identity;

namespace Dashboard.Controllers.API
{
    [Authorize(Roles = DashboardRoles.Admin)]
    [RoutePrefix("api/roles")]
    public class RolesController : BaseController
    {
        [Route("get/{id:guid}", Name = "GetRoleById")]
        public async Task<IHttpActionResult> GetRole(string id)
        {
            var role = await RoleManager.FindByIdAsync(id);
            if (role == null) return NotFound();

            return Ok(new { id = role.Id, name = role.Name, users = role.Users.Select(p => p.UserId) });
        }

        [Route("", Name = "GetAllRoles")]
        public IHttpActionResult GetAllRoles()
        {
            var roles = RoleManager.Roles;
            return Ok(roles);
        }

        [Route("users/in/{role:guid}")]
        public async Task<IHttpActionResult> GetUsersInRole(string role)
        {
            var userRole = await RoleManager.FindByIdAsync(role);
            ModelState.AddModelError("", "Role not found");

            if (role == null) return BadRequest(ModelState);

            var allUsers = UserManager.Users.ToList();
            var usersInRole = userRole.Users.Select(p => UserManager.FindById(p.UserId));
            return Ok(new { inRoleUsers = usersInRole, notAssignedUsers = allUsers });
        }

        [Route("assign/roles", Name = "ManageUsersInRole")]
        public async Task<IHttpActionResult> ManageUsersInRole([FromBody] UsersInRoleModel model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var role = await RoleManager.FindByIdAsync(model.RoleId.ToString());

            if (role == null)
            {
                ModelState.AddModelError("", "Role does not exist");
                return BadRequest(ModelState);
            }

            foreach (var user in model.EnrolledUsers.Select(p => p.ToString()))
            {
                var appUser = await RoleManager.FindByIdAsync(user);

                if (appUser == null)
                {
                    ModelState.AddModelError("", $"User: {user} does not exists");
                    continue;
                }

                if (UserManager.IsInRole(user, role.Name)) continue;

                var result = await UserManager.AddToRoleAsync(user, role.Name);

                if (!result.Succeeded)
                {
                    ModelState.AddModelError("", $"User: {user} could not be added to role");
                }
            }

            foreach (var user in model.RemovedUsers.Select(p => p.ToString()))
            {
                var appUser = await UserManager.FindByIdAsync(user);

                if (appUser == null)
                {
                    ModelState.AddModelError("", $"User: {user} does not exists");
                    continue;
                }

                if (!UserManager.IsInRole(user, role.Name)) continue;

                var result = await UserManager.RemoveFromRoleAsync(user, role.Name);

                if (!result.Succeeded)
                {
                    ModelState.AddModelError("", $"User: {user} could not be removed from role");
                }
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return Ok();
        }
    }
}

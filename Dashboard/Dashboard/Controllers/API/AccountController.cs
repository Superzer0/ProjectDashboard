using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http;
using Common.Logging;
using Dashboard.Infrastructure.Controllers;
using Dashboard.Models.Account;
using Microsoft.AspNet.Identity;

namespace Dashboard.Controllers.API
{
    [Authorize]
    [RoutePrefix("api/account")]
    public class AccountController : BaseController
    {
        private readonly ILog _log = LogManager.GetLogger<AccountController>();

        [HttpPost]
        [AllowAnonymous]
        [Route("register", Name = "RegisterUser")]
        public async Task<IHttpActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var user = new DashboardUser { UserName = model.UserName, Email = model.Email };
            var result = await UserManager.CreateAsync(user, model.Password);

            var validationResult = GetErrorResult(result);
            if (validationResult != null) return validationResult;

            _log.Info(p => p("Registered User " + user.UserName));
            return Ok();
        }

        [Route("user-info", Name = "UserInfo")]
        [Authorize]
        public IHttpActionResult GetUserInfo()
        {
            var identity = User.Identity as ClaimsIdentity;
            return Ok(identity?.Claims.Select(p => new { subject = p.Subject.Name, type = p.Type, value = p.Value }));
        }

        [HttpPost]
        [Route("logoff", Name = "LogOffUser")]
        public IHttpActionResult LogOff()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return Ok();
        }

        private IHttpActionResult GetErrorResult(IdentityResult result)
        {
            if (result == null) return InternalServerError();

            if (result.Succeeded) return null; // everything ok

            if (result.Errors != null)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error);
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

using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http;
using Common.Logging;
using Dashboard.Infrastructure.Controllers;
using Dashboard.Models.Account;
using Dashboard.Models.Home;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;

namespace Dashboard.Controllers.API
{
    [Authorize]
    [RoutePrefix("api/account")]
    public class AccountController : RazorController
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private readonly ILog _log = LogManager.GetLogger<AccountController>();

        public AccountController() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="AccountController"/> class.
        /// Created for Unit Tests
        /// </summary>
        /// <param name="userManager">The user manager.</param>
        /// <param name="signInManager">The sign in manager.</param>
        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("register", Name = "RegisterUser")]
        public async Task<IHttpActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var user = new DashboardUser { UserName = model.Email, Email = model.Email };
            var result = await UserManager.CreateAsync(user, model.Password);

            var validationResult = GetErrorResult(result);
            if (validationResult != null) return validationResult;

            await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);

            //Dorobić wysyłanie email po zalogowaniu później:
            // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
            // Send an email with this link
            // string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
            // var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
            // await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");

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

        [Route("claims")]
        [Authorize]
        public IHttpActionResult GetClaims()
        {
            var identity = User.Identity as ClaimsIdentity;
            return Ok(identity?.Claims.Select(p => new { subject = p.Subject.Name, type = p.Type, value = p.Value }));
        }


        // The Authorize Action is the end point which gets called when you access any
        // protected Web API. If the user is not logged in then they will be redirected to 
        // the Login page. After a successful login you can call a Web API.
        [HttpGet]
        public IHttpActionResult Authorize()
        {
            var claims = new ClaimsPrincipal(User).Claims.ToArray();
            var identity = new ClaimsIdentity(claims, "Bearer");
            AuthenticationManager.SignIn(identity);
            return Ok();
        }

        //
        // GET: /Account/Login
        [AllowAnonymous]
        [HttpGet]
        [Route("login", Name = "LoginUserView")]
        public IHttpActionResult Login(string returnUrl)
        {
            return View("../Views/Home.cshtml", new HomeViewModel());
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [Route("login", Name = "LoginUser")]
        public async Task<IHttpActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.BadRequest,
                    ModelState.Values.SelectMany(p => p.Errors)));
            }

            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, change to shouldLockout: true
            var result = await SignInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, shouldLockout: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return Redirect(returnUrl);
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid login attempt.");
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.BadRequest,
                    ModelState.Values.SelectMany(p => p.Errors)));
            }
        }

        [HttpPost]
        [Route("logoff", Name = "LogOffUser")]
        public IHttpActionResult LogOff()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToRoute("HomeRoute", new { controller = "Home" });
        }

        private IAuthenticationManager AuthenticationManager => Request.GetOwinContext().Authentication;

        private ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? Request.GetOwinContext().Get<ApplicationSignInManager>();
            }
            set
            {
                _signInManager = value;
            }
        }

        private ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? Request.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            set
            {
                _userManager = value;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
            }

            base.Dispose(disposing);
        }
    }
}

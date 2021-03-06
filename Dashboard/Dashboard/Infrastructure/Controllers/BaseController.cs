﻿using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Dashboard.Infrastructure.Identity.Managers;
using Dashboard.UI.Objects.Auth;
using Dashboard.UI.Objects.Services;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;

namespace Dashboard.Infrastructure.Controllers
{
    public class BaseController : ApiController
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private IAuthenticationManager _authenticationManager;
        private ApplicationRoleManager _roleManager;
        //public properties for unit testing
        // ReSharper disable  MemberCanBeProtected.Global
        public IEnvironment Environment { get; set; } // DI injected

        public IAuthenticationManager AuthenticationManager
        {
            get { return _authenticationManager = _authenticationManager ?? Request.GetOwinContext().Authentication; }
            set { _authenticationManager = value; }
        }

        public ApplicationRoleManager RoleManager
        {
            get { return _roleManager = _roleManager ?? Request.GetOwinContext().Get<ApplicationRoleManager>(); }
            set { _roleManager = value; }
        }

        public ApplicationSignInManager SignInManager
        {
            get { return _signInManager = _signInManager ?? Request.GetOwinContext().Get<ApplicationSignInManager>(); }
            set { _signInManager = value; }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager = _userManager ?? Request.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            set { _userManager = value; }
        }

        protected async Task<DashboardUser> GetCurrentUser()
        {
            return await UserManager.FindByNameAsync(User.Identity.Name);
        }

        protected string GetModelStateSummary()
        {
            return string.Join(", ", ModelState.Values.SelectMany(p => p.Errors).Select(p => p.ErrorMessage).ToList());
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

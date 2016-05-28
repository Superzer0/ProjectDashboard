using System.Security.Claims;
using System.Threading.Tasks;
using Autofac;
using Autofac.Integration.Owin;
using Dashboard.UI.Objects.Auth;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;

namespace Dashboard.Infrastructure.Identity.Managers
{
    // Configure the application sign-in manager which is used in this application.  
    public class ApplicationSignInManager : SignInManager<DashboardUser, string>
    {
        public ApplicationSignInManager(ApplicationUserManager userManager, IAuthenticationManager authenticationManager) :
            base(userManager, authenticationManager)
        { }

        public override Task<ClaimsIdentity> CreateUserIdentityAsync(DashboardUser user)
        {
            return user.GenerateUserCookieIdentityAsync((ApplicationUserManager)UserManager);
        }

        public static ApplicationSignInManager Create(IdentityFactoryOptions<ApplicationSignInManager> options, IOwinContext context)
        {
            //TEMPORARY: cannot pass IOwinContext through autofac :/
            return new ApplicationSignInManager(context.GetAutofacLifetimeScope().Resolve<ApplicationUserManager>(), context.Authentication);
        }
    }
}

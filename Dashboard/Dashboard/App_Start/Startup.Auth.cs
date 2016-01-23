using System;
using System.Linq;
using Common.Logging;
using Dashboard.Infrastructure.Identity;
using Dashboard.UI.Objects.Auth;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OAuth;
using Owin;

namespace Dashboard
{
    public partial class Application
    {
        private void ConfigureAuth(IAppBuilder app)
        {
            // Configure the db context, user manager and signin manager to use a single instance per request
            app.CreatePerOwinContext(AuthContext.Create);
            app.CreatePerOwinContext<ApplicationUserManager>(ApplicationUserManager.Create);
            app.CreatePerOwinContext<ApplicationSignInManager>(ApplicationSignInManager.Create);
            app.CreatePerOwinContext<ApplicationRoleManager>(ApplicationRoleManager.Create);

            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                Provider = new CookieAuthenticationProvider
                {
                    // Enables the application to validate the security stamp when the user logs in.
                    // This is a security feature which is used when you change a password or add an external login to your account.  
                    OnValidateIdentity = SecurityStampValidator.OnValidateIdentity<ApplicationUserManager, DashboardUser>(
                        validateInterval: TimeSpan.FromMinutes(20),
                        regenerateIdentity: (manager, user) => user.GenerateUserIdentityAsync(manager))
                }
            });

            var authAuthorizationServerOptions = new OAuthAuthorizationServerOptions
            {
                AllowInsecureHttp = true,
                TokenEndpointPath = new PathString("/token"),
                AccessTokenExpireTimeSpan = TimeSpan.FromHours(4),
                Provider = new SimpleAuthorizationServerProvider()
            };

            app.UseOAuthAuthorizationServer(authAuthorizationServerOptions);
            app.UseOAuthBearerAuthentication(new OAuthBearerAuthenticationOptions());
        }

        private void AddStandardRoles()
        {
            try
            {
                LogManager.GetLogger<Application>().Info(m => m("Checking standard roles..."));
                var roleManager = new ApplicationRoleManager(new RoleStore<IdentityRole>(new AuthContext()));
                if (roleManager.Roles.Any()) return;

                roleManager.Create(new IdentityRole(DashboardRoles.User));
                roleManager.Create(new IdentityRole(DashboardRoles.Admin));
                roleManager.Create(new IdentityRole(DashboardRoles.PluginManager));
                LogManager.GetLogger<Application>().Info(m => m("Created standard roles"));
            }
            catch (Exception e)
            {
                LogManager.GetLogger<Application>().Error(e);
                LogManager.GetLogger<Application>().Error(m => m("Error while creating standard roles", e));
            }
        }
    }
}

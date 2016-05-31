using System;
using Dashboard.Infrastructure.Identity.Managers;
using Dashboard.Infrastructure.Identity.Server;
using Dashboard.UI.Objects.Auth;
using Microsoft.AspNet.Identity;
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
            app.CreatePerOwinContext<ApplicationSignInManager>(ApplicationSignInManager.Create);

            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                ExpireTimeSpan = TimeSpan.FromMinutes(5),
                LoginPath = new PathString("/"),
                Provider = new CookieAuthenticationProvider
                {
                    // Enables the application to validate the security stamp when the user logs in.
                    // This is a security feature which is used when you change a password or add an external login to your account.  
                    OnValidateIdentity = SecurityStampValidator.OnValidateIdentity<ApplicationUserManager, DashboardUser>(
                       validateInterval: TimeSpan.FromMinutes(5),
                       regenerateIdentity: (manager, user) => user.GenerateUserBearerIdentityAsync(manager))
                }
            });

            var authAuthorizationServerOptions = new OAuthAuthorizationServerOptions
            {
                AllowInsecureHttp = true,
                TokenEndpointPath = new PathString("/token"),
                AccessTokenExpireTimeSpan = TimeSpan.FromMinutes(5),
                Provider = new SimpleAuthorizationServerProvider(),
                RefreshTokenProvider = new SimpleRefreshTokenProvider()
            };

            app.UseOAuthAuthorizationServer(authAuthorizationServerOptions);
            app.UseOAuthBearerAuthentication(new OAuthBearerAuthenticationOptions());
        }
    }
}

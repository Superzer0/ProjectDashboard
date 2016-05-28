using System;
using Dashboard.Infrastructure.Identity.Managers;
using Dashboard.Infrastructure.Identity.Server;
using Microsoft.Owin;
using Microsoft.Owin.Security.OAuth;
using Owin;

namespace Dashboard
{
    public partial class Application
    {
        private void ConfigureAuth(IAppBuilder app)
        {
            app.CreatePerOwinContext<ApplicationSignInManager>(ApplicationSignInManager.Create);

            var authAuthorizationServerOptions = new OAuthAuthorizationServerOptions
            {
                AllowInsecureHttp = true,
                TokenEndpointPath = new PathString("/token"),
                AccessTokenExpireTimeSpan = TimeSpan.FromMinutes(3),
                Provider = new SimpleAuthorizationServerProvider(),
                RefreshTokenProvider = new SimpleRefreshTokenProvider()
            };

            app.UseOAuthAuthorizationServer(authAuthorizationServerOptions);
            app.UseOAuthBearerAuthentication(new OAuthBearerAuthenticationOptions());
        }
    }
}

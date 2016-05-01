using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Autofac;
using Autofac.Integration.Owin;
using Common.Logging;
using Dashboard.UI.Objects.Auth;
using Dashboard.UI.Objects.Providers;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;

namespace Dashboard.Infrastructure.Identity
{
    internal class SimpleAuthorizationServerProvider : OAuthAuthorizationServerProvider
    {
        private readonly ILog _logger = LogManager.GetLogger<SimpleAuthorizationServerProvider>();
        public override async Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            context.Validated();
        }

        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            try
            {
                context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { "*" });

                var authRepository = Resolve<AuthRepository>(context);
                var user = await authRepository.FindUser(context.UserName, context.Password);
                if (user == null)
                {
                    context.SetError("invalid_grant", "The user name or password is incorrect");
                    return;
                }

                AuthenticationManager(context).SignOut();

                await SignInManager(context).SignInAsync(user, false, false);
                var userIdentity = await authRepository.GetUserClaims(user);

                userIdentity.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.Id));
                userIdentity.AddClaim(new Claim(ClaimTypes.Name, context.UserName));
                userIdentity.AddClaim(new Claim(ClaimTypes.Email, user.Email));
                AddAdminPartyRoles(context, userIdentity);

                context.Validated(userIdentity);
            }
            catch (Exception e)
            {
                _logger.Error(e);
                throw;
            }
        }

        private static void AddAdminPartyRoles(OAuthGrantResourceOwnerCredentialsContext context, ClaimsIdentity userIdentity)
        {
            var configureDashboard = Resolve<IConfigureDashboard>(context);

            if (!configureDashboard.GetAdminPartyState()) return;

            userIdentity.AddClaim(new Claim(ClaimTypes.Role, DashboardRoles.User));
            userIdentity.AddClaim(new Claim(ClaimTypes.Role, DashboardRoles.PluginManager));
            userIdentity.AddClaim(new Claim(ClaimTypes.Role, DashboardRoles.Admin));
        }

        private ApplicationSignInManager SignInManager(OAuthGrantResourceOwnerCredentialsContext context)
        {
            return context.OwinContext.Get<ApplicationSignInManager>();
        }

        private IAuthenticationManager AuthenticationManager(OAuthGrantResourceOwnerCredentialsContext context)
        {
            return context.OwinContext.Authentication;
        }

        private ApplicationUserManager GetUserManager(OAuthGrantResourceOwnerCredentialsContext context)
        {
            return context.OwinContext.GetUserManager<ApplicationUserManager>();
        }
        private static T Resolve<T>(OAuthGrantResourceOwnerCredentialsContext context)
        {
            var container = context.OwinContext.GetAutofacLifetimeScope();
            return container.Resolve<T>();
        }
    }
}

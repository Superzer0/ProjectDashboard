using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security.OAuth;

namespace Dashboard.Infrastructure.Identity
{
    internal class SimpleAuthorizationServerProvider : OAuthAuthorizationServerProvider
    {
        public override async Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            context.Validated();
        }

        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { "*" });

            using (var authRepository = new AuthRepository())
            {
                var user = await authRepository.FindUser(context.UserName, context.Password);
                if (user == null)
                {
                    context.SetError("invalid_grant", "The user name or password is incorrect");
                    return;
                }

                await GetSignInManager(context).SignInAsync(user, false, false);
                var userIdentity = await authRepository.GetUserClaims(user);
                //userIdentity.AuthenticationType = context.Options.AuthenticationType;
                userIdentity.AddClaim(new Claim("sub", context.UserName));
                userIdentity.AddClaim(new Claim("role", "user"));

                context.Validated(userIdentity);
            }
        }
        private ApplicationSignInManager GetSignInManager(OAuthGrantResourceOwnerCredentialsContext context)
        {
            return context.OwinContext.Get<ApplicationSignInManager>();
        }

        private ApplicationUserManager GetUserManager(OAuthGrantResourceOwnerCredentialsContext context)
        {
            return context.OwinContext.GetUserManager<ApplicationUserManager>();
        }
    }
}

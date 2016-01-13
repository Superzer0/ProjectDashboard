using System.Net.Mime;
using System.Security.Claims;
using System.Threading.Tasks;
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

                var userIdentity = await authRepository.GetUserClaims(user);
                //userIdentity.AuthenticationType = context.Options.AuthenticationType;
                userIdentity.AddClaim(new Claim("sub", context.UserName));
                userIdentity.AddClaim(new Claim("role", "user"));

                context.Validated(userIdentity);
            }
        }
    }
}

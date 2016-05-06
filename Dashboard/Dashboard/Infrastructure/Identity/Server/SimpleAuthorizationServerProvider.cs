using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Autofac;
using Autofac.Integration.Owin;
using Common.Logging;
using Dashboard.Infrastructure.Identity.Managers;
using Dashboard.UI.Objects.Auth;
using Dashboard.UI.Objects.Providers;
using Dashboard.UI.Objects.Services;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;

namespace Dashboard.Infrastructure.Identity.Server
{
    internal class SimpleAuthorizationServerProvider : OAuthAuthorizationServerProvider
    {
        private readonly ILog _logger = LogManager.GetLogger<SimpleAuthorizationServerProvider>();

        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            try
            {
                var allowedOrigin = context.OwinContext.Get<string>("as:clientAllowedOrigin") ?? "*";

                context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { allowedOrigin });

                var authRepository = Resolve<IAuthRepository>(context.OwinContext);
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

                var props = new AuthenticationProperties(new Dictionary<string, string>
                {
                    {"as:client_id", context.ClientId ?? string.Empty},
                    {"userName", context.UserName}
                });

                var ticket = new AuthenticationTicket(userIdentity, props);

                context.Validated(ticket);
            }
            catch (Exception e)
            {
                _logger.Error(e);
                throw;
            }
        }

        public override Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            string clientId;
            string clientSecret;

            if (!context.TryGetBasicCredentials(out clientId, out clientSecret))
            {
                context.TryGetFormCredentials(out clientId, out clientSecret);
            }

            if (context.ClientId == null)
            {
                //Remove the comments from the below line context.SetError, and invalidate context 
                //if you want to force sending clientId/secrects once obtain access tokens. 
                context.Validated();
                //context.SetError("invalid_clientId", "ClientId should be sent.");
                return Task.FromResult<object>(null);
            }

            var authRepository = Resolve<IAuthRepository>(context.OwinContext);
            var client = authRepository.FindClient(context.ClientId);

            if (client == null)
            {
                context.SetError("invalid_clientId", $"Client '{context.ClientId}' is not registered in the system.");
                return Task.FromResult<object>(null);
            }

            if (client.ApplicationType == AuthApplicationType.NativeConfidential)
            {
                if (string.IsNullOrWhiteSpace(clientSecret))
                {
                    context.SetError("invalid_clientId", "Client secret should be sent.");
                    return Task.FromResult<object>(null);
                }

                if (client.Secret != authRepository.GetHash(clientSecret))
                {
                    context.SetError("invalid_clientId", "Client secret is invalid.");
                    return Task.FromResult<object>(null);
                }
            }

            if (!client.Active)
            {
                context.SetError("invalid_clientId", "Client is inactive.");
                return Task.FromResult<object>(null);
            }

            context.OwinContext.Set("as:clientAllowedOrigin", client.AllowedOrigin);
            context.OwinContext.Set("as:clientRefreshTokenLifeTime", client.RefreshTokenLifeTime.ToString());

            context.Validated();
            return Task.FromResult<object>(null);
        }

        public override Task GrantRefreshToken(OAuthGrantRefreshTokenContext context)
        {
            var originalClient = context.Ticket.Properties.Dictionary["as:client_id"];
            var currentClient = context.ClientId;

            if (originalClient != currentClient)
            {
                context.SetError("invalid_clientId", "Refresh token is issued to a different clientId.");
                return Task.FromResult<object>(null);
            }

            // Change auth ticket for refresh token requests
            var newIdentity = new ClaimsIdentity(context.Ticket.Identity);
            newIdentity.AddClaim(new Claim("newClaim", "newValue"));

            var newTicket = new AuthenticationTicket(newIdentity, context.Ticket.Properties);
            context.Validated(newTicket);

            return Task.FromResult<object>(null);
        }

        private static void AddAdminPartyRoles(OAuthGrantResourceOwnerCredentialsContext context,
            ClaimsIdentity userIdentity)
        {
            var configureDashboard = Resolve<IConfigureDashboard>(context.OwinContext);

            if (!configureDashboard.GetAdminPartyState()) return;

            userIdentity.AddClaim(new Claim(ClaimTypes.Role, DashboardRoles.User));
            userIdentity.AddClaim(new Claim(ClaimTypes.Role, DashboardRoles.PluginManager));
            userIdentity.AddClaim(new Claim(ClaimTypes.Role, DashboardRoles.Admin));
        }

        public override Task TokenEndpoint(OAuthTokenEndpointContext context)
        {
            foreach (KeyValuePair<string, string> property in context.Properties.Dictionary)
            {
                context.AdditionalResponseParameters.Add(property.Key, property.Value);
            }

            return Task.FromResult<object>(null);
        }

        private ApplicationSignInManager SignInManager(OAuthGrantResourceOwnerCredentialsContext context)
        {
            return context.OwinContext.Get<ApplicationSignInManager>();
        }

        private IAuthenticationManager AuthenticationManager(OAuthGrantResourceOwnerCredentialsContext context)
        {
            return context.OwinContext.Authentication;
        }

        private static T Resolve<T>(IOwinContext context)
        {
            var container = context.GetAutofacLifetimeScope();
            return container.Resolve<T>();
        }
    }
}

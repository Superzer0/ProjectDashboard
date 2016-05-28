using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Autofac;
using Autofac.Integration.Owin;
using Common.Logging;
using Dashboard.UI.Objects.Auth;
using Dashboard.UI.Objects.Providers;
using Dashboard.UI.Objects.Services;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;

namespace Dashboard.Infrastructure.Identity.Server
{
    internal class SimpleAuthorizationServerProvider : OAuthAuthorizationServerProvider
    {
        private const string DefaultAllowedOrigin = "*";
        private readonly ILog _logger = LogManager.GetLogger<SimpleAuthorizationServerProvider>();

        /// <summary>
        ///  Validates user agent that is acting on behalf of the user
        /// </summary>
        public override async Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            string clientId;
            string clientSecret;

            if (!context.TryGetBasicCredentials(out clientId, out clientSecret))
            {
                context.TryGetFormCredentials(out clientId, out clientSecret);
            }

            if (context.ClientId == null)
            {
                context.Validated();
                SetContextEror(context, AuthError.AgentErrorClientIdMissing);
                return;
            }

            var authRepository = Resolve<IAuthRepository>(context.OwinContext);
            var client = await authRepository.FindClient(context.ClientId);

            if (client == null)
            {
                SetContextEror(context, AuthError.AgentErrorClientIdNotFound);
                return;
            }

            if (client.ApplicationType == AuthApplicationType.NativeConfidential)
            {
                if (string.IsNullOrWhiteSpace(clientSecret))
                {
                    SetContextEror(context, AuthError.AgentErrorSecretMissing);
                    return;
                }

                if (client.Secret != authRepository.GetHash(clientSecret))
                {
                    SetContextEror(context, AuthError.AgentErrorSecretInvalid);
                    return;
                }
            }

            if (!client.Active)
            {
                SetContextEror(context, AuthError.AgentErrorAppInactive);
                return;
            }

            context.OwinContext.Set(AuthContextParameters.AllowedOrigin, client.AllowedOrigin);
            context.OwinContext.Set(AuthContextParameters.AllowedRefreshTokenLifeTime, client.RefreshTokenLifeTime.ToString());

            context.Validated();
        }
        /// <summary>
        /// Validates user based on sent user_name/password. Creates auth ticket
        /// </summary>
        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            try
            {
                var allowedOrigin = context.OwinContext.Get<string>(AuthContextParameters.AllowedOrigin) ?? DefaultAllowedOrigin;

                context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { allowedOrigin });

                var authRepository = Resolve<IAuthRepository>(context.OwinContext);
                var user = await authRepository.FindUser(context.UserName, context.Password);
                var userIdentity = await BuildUserIdentity(context, authRepository, user);

                if (userIdentity == null)
                {
                    SetContextEror(context, AuthError.PasswordGrantCredentialsInvalid);
                    return;
                }

                var props = new AuthenticationProperties(new Dictionary<string, string>
                {
                    {AuthContextParameters.ClientId, context.ClientId ?? string.Empty},
                    {AuthContextParameters.UserNameKey, context.UserName}
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

        /// <summary>
        /// Restores user identity based on refresh token id, 
        /// </summary>
        public override async Task GrantRefreshToken(OAuthGrantRefreshTokenContext context)
        {
            var originalClient = context.Ticket.Properties.Dictionary[AuthContextParameters.ClientId];
            var currentClient = context.ClientId;

            if (originalClient != currentClient)
            {
                SetContextEror(context, AuthError.AgentErrorRefreshTokenMismatch);
            }

            var authRepository = Resolve<IAuthRepository>(context.OwinContext);
            var user = await authRepository.FindUser(context.Ticket.Identity.Name);
            var newUserIdentity = await BuildUserIdentity(context, authRepository, user);

            if (newUserIdentity == null)
            {
                SetContextEror(context, AuthError.UserNotFoundForRefreshToken);
                return;
            }

            var newTicket = new AuthenticationTicket(newUserIdentity, context.Ticket.Properties);
            context.Validated(newTicket);
        }

        public override async Task TokenEndpoint(OAuthTokenEndpointContext context)
        {
            foreach (var property in context.Properties.Dictionary)
            {
                context.AdditionalResponseParameters.Add(property.Key, property.Value);
            }
        }

        private async Task<ClaimsIdentity> BuildUserIdentity(BaseValidatingTicketContext<OAuthAuthorizationServerOptions> context,
            IAuthRepository authRepository, DashboardUser user)
        {
            if (context == null) throw new NullReferenceException("context must not be null");
            if (authRepository == null) throw new NullReferenceException("authRepository must not be null");
            if (user == null) return null;

            var userIdentity = await authRepository.GetUserClaims(user);
            userIdentity.AddClaim(new Claim(ClaimTypes.Email, user.Email));
            AddAdminPartyRoles(context, userIdentity);
            return userIdentity;
        }

        private static void AddAdminPartyRoles(BaseValidatingTicketContext<OAuthAuthorizationServerOptions> context,
            ClaimsIdentity userIdentity)
        {
            var configureDashboard = Resolve<IConfigureDashboard>(context.OwinContext);

            if (!configureDashboard.GetAdminPartyState()) return;
            var userClaim = new Claim(ClaimTypes.Role, DashboardRoles.User);
            var pluginManagerClaim = new Claim(ClaimTypes.Role, DashboardRoles.PluginManager);
            var adminClaim = new Claim(ClaimTypes.Role, DashboardRoles.Admin);

            foreach (var claim in userIdentity.Claims.Where(p => p.Type == ClaimTypes.Role))
            {
                userIdentity.TryRemoveClaim(claim);
            }

            userIdentity.AddClaim(userClaim);
            userIdentity.AddClaim(pluginManagerClaim);
            userIdentity.AddClaim(adminClaim);
        }

        private void SetContextEror(BaseValidatingClientContext context, AuthErrorDescription authError)
        {
            context.SetError(authError.ErrorKey, authError.ErrorDescription);
        }

        private void SetContextEror<T>(BaseValidatingContext<T> context, AuthErrorDescription authError)
        {
            context.SetError(authError.ErrorKey, authError.ErrorDescription);
        }

        private static T Resolve<T>(IOwinContext context)
        {
            var container = context.GetAutofacLifetimeScope();
            return container.Resolve<T>();
        }
    }
}

using System;
using System.Threading.Tasks;
using Autofac;
using Autofac.Integration.Owin;
using Dashboard.UI.Objects.Auth;
using Dashboard.UI.Objects.Services;
using Microsoft.Owin;
using Microsoft.Owin.Security.Infrastructure;

namespace Dashboard.Infrastructure.Identity.Server
{
    public class SimpleRefreshTokenProvider : IAuthenticationTokenProvider
    {
        public void Create(AuthenticationTokenCreateContext context)
        {
            Task.WaitAll(CreateAsync(context));
        }

        public async Task CreateAsync(AuthenticationTokenCreateContext context)
        {
            var clientid = context.Ticket.Properties.Dictionary["as:client_id"];

            if (string.IsNullOrEmpty(clientid))
            {
                return;
            }

            var refreshTokenId = Guid.NewGuid().ToString("n");

            var refreshTokenLifeTime = context.OwinContext.Get<string>("as:clientRefreshTokenLifeTime");
            var authRepository = Resolve<IAuthRepository>(context.OwinContext);
            var token = new AuthRefreshToken
            {
                Id = authRepository.GetHash(refreshTokenId),
                ClientId = clientid,
                Subject = context.Ticket.Identity.Name,
                IssuedUtc = DateTime.UtcNow,
                ExpiresUtc = DateTime.UtcNow.AddMinutes(Convert.ToDouble(refreshTokenLifeTime))
            };

            context.Ticket.Properties.IssuedUtc = token.IssuedUtc;
            context.Ticket.Properties.ExpiresUtc = token.ExpiresUtc;

            token.ProtectedTicket = context.SerializeTicket();

            var result = await authRepository.AddRefreshToken(token);

            if (result)
            {
                context.SetToken(refreshTokenId);
            }
        }

        public void Receive(AuthenticationTokenReceiveContext context)
        {
            Task.WaitAll(ReceiveAsync(context));
        }

        public async Task ReceiveAsync(AuthenticationTokenReceiveContext context)
        {
            var allowedOrigin = context.OwinContext.Get<string>("as:clientAllowedOrigin");
            context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { allowedOrigin });

            var authRepository = Resolve<IAuthRepository>(context.OwinContext);
            var hashedTokenId = authRepository.GetHash(context.Token);
            
            var refreshToken = await authRepository.FindRefreshToken(hashedTokenId);

            if (refreshToken != null)
            {
                //Get protectedTicket from refreshToken class
                context.DeserializeTicket(refreshToken.ProtectedTicket);
                var result = await authRepository.RemoveRefreshToken(hashedTokenId);
            }
        }

        private static T Resolve<T>(IOwinContext context)
        {
            var container = context.GetAutofacLifetimeScope();
            return container.Resolve<T>();
        }
    }
}

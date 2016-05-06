using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Dashboard.UI.Objects.Auth;

namespace Dashboard.UI.Objects.Services
{
    public interface IAuthRepository
    {
        Task<DashboardUser> FindUser(string userName, string password);
        Task<ClaimsIdentity> GetUserClaims(DashboardUser user);
        AuthClient FindClient(string clientId);
        Task<bool> AddRefreshToken(AuthRefreshToken token);
        Task<bool> RemoveRefreshToken(string refreshTokenId);
        Task<bool> RemoveRefreshToken(AuthRefreshToken refreshToken);
        Task<AuthRefreshToken> FindRefreshToken(string refreshTokenId);
        List<AuthRefreshToken> GetAllRefreshTokens();
        string GetHash(string input);
        Tuple<AuthClient, string> CreateClient(string name, AuthApplicationType applicationType, string allowedOrigin);
        IEnumerable<AuthClient> GetAllClients();
    }
}

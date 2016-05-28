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
        Task<DashboardUser> FindUser(string userName);
        Task<ClaimsIdentity> GetUserClaims(DashboardUser user);
        Task<AuthClient> FindClient(string clientId);
        Task<bool> AddRefreshToken(AuthRefreshToken token);
        Task<bool> RemoveRefreshToken(string refreshTokenId);
        Task<bool> RemoveRefreshToken(AuthRefreshToken refreshToken);
        Task<AuthRefreshToken> FindRefreshToken(string refreshTokenId);
        List<AuthRefreshToken> GetAllRefreshTokens();
        string GetHash(string input);
        AuthClient CreateClient(string name, AuthApplicationType applicationType, string allowedOrigin);
        IEnumerable<AuthClient> GetAllClients();
        AuthClient GenerateOfficialClientId();
        Task<bool> DeleteClient(string appId);
        Task<bool> ToggleAppState(string appId, bool isActive);
        Task<string> ReGenerateAppSecret(string appId);
    }
}

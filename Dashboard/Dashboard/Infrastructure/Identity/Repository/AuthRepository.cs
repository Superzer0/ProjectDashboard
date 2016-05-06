using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Common.Logging;
using Dashboard.DataAccess;
using Dashboard.Infrastructure.Identity.Managers;
using Dashboard.UI.Objects.Auth;
using Dashboard.UI.Objects.Services;

namespace Dashboard.Infrastructure.Identity.Repository
{
    public class AuthRepository : IAuthRepository
    {
        private readonly ILog _log = LogManager.GetLogger<AuthRepository>();
        private readonly ApplicationUserManager _userManager;
        private readonly AuthDbContext _authDbContext;

        public AuthRepository(ApplicationUserManager userManager, AuthDbContext authDbContext)
        {
            _userManager = userManager;
            _authDbContext = authDbContext;
        }

        public async Task<DashboardUser> FindUser(string userName, string password)
        {
            try
            {
                var user = await _userManager.FindAsync(userName, password);
                return user;
            }
            catch (Exception e)
            {
                _log.Error(e);
                throw;
            }
        }

        public async Task<ClaimsIdentity> GetUserClaims(DashboardUser user)
        {
            return await user.GenerateUserIdentityAsync(_userManager);
        }

        public AuthClient FindClient(string clientId)
        {
            var client = _authDbContext.Clients.Find(clientId);

            return client;
        }

        public async Task<bool> AddRefreshToken(AuthRefreshToken token)
        {

            var existingToken =
                _authDbContext.RefreshTokens.SingleOrDefault(
                    r => r.Subject == token.Subject && r.ClientId == token.ClientId);

            if (existingToken != null)
            {
                var result = await RemoveRefreshToken(existingToken);
            }

            _authDbContext.RefreshTokens.Add(token);

            return await _authDbContext.SaveChangesAsync() > 0;
        }

        public async Task<bool> RemoveRefreshToken(string refreshTokenId)
        {
            var refreshToken = await _authDbContext.RefreshTokens.FindAsync(refreshTokenId);

            if (refreshToken == null) return false;

            _authDbContext.RefreshTokens.Remove(refreshToken);
            return await _authDbContext.SaveChangesAsync() > 0;
        }

        public async Task<bool> RemoveRefreshToken(AuthRefreshToken refreshToken)
        {
            _authDbContext.RefreshTokens.Remove(refreshToken);
            return await _authDbContext.SaveChangesAsync() > 0;
        }

        public async Task<AuthRefreshToken> FindRefreshToken(string refreshTokenId)
        {
            var refreshToken = await _authDbContext.RefreshTokens.FindAsync(refreshTokenId);

            return refreshToken;
        }

        public List<AuthRefreshToken> GetAllRefreshTokens()
        {
            return _authDbContext.RefreshTokens.ToList();
        }

        public string GetHash(string input)
        {
            using (HashAlgorithm hashAlgorithm = new SHA256CryptoServiceProvider())
            {
                var byteValue = System.Text.Encoding.UTF8.GetBytes(input);
                var byteHash = hashAlgorithm.ComputeHash(byteValue);
                return Convert.ToBase64String(byteHash);
            }
        }

        public Tuple<AuthClient, string> CreateClient(string name, AuthApplicationType applicationType, string allowedOrigin)
        {
            var appId = Guid.NewGuid().ToString("N");
            var appSecret = $"{Guid.NewGuid().ToString("N")}{Guid.NewGuid().ToString("N")}";
            var origin = applicationType == AuthApplicationType.NativeConfidential ? "*" : allowedOrigin;
            if (string.IsNullOrWhiteSpace(origin))
                throw new ArgumentException("allowed origin must not be null", nameof(allowedOrigin));

            var authClient = new AuthClient
            {
                Id = appId,
                Name = name,
                ApplicationType = applicationType,
                AllowedOrigin = origin,
                CreatedAt = DateTime.Now,
                Active = true,
                RefreshTokenLifeTime = DefaultRefreshTokenExpiration[applicationType],
                Secret = GetHash(appSecret)
            };

            _authDbContext.Clients.Add(authClient);
            _authDbContext.SaveChanges();

            return new Tuple<AuthClient, string>(authClient, appSecret);
        }

        public IEnumerable<AuthClient> GetAllClients()
        {
            return _authDbContext.Clients.ToList();
        }

        private static readonly Dictionary<AuthApplicationType, int> DefaultRefreshTokenExpiration = new Dictionary<AuthApplicationType, int>
        {
            {AuthApplicationType.JavaScript, 7200 },
            {AuthApplicationType.NativeConfidential, 14440 }
        };
    }
}

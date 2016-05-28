using System;
using System.Collections.Generic;
using System.Data.Entity;
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
        private readonly IEnvironment _environment;
        private const string OfficialAppName = "Official Web Browser Client";
        private static volatile AuthClient _officialJsClient;
        private static readonly object Lock = new object();

        public AuthRepository(ApplicationUserManager userManager, AuthDbContext authDbContext, IEnvironment environment)
        {
            _userManager = userManager;
            _authDbContext = authDbContext;
            _environment = environment;
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

        public async Task<DashboardUser> FindUser(string userName)
        {
            try
            {
                var user = await _userManager.FindByNameAsync(userName);
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
            return await user.GenerateUserBearerIdentityAsync(_userManager);
        }

        public async Task<AuthClient> FindClient(string clientId)
        {
            var client = await _authDbContext.Clients.FindAsync(clientId);
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
            return _authDbContext.RefreshTokens.OrderByDescending(p => p.ExpiresUtc).ToList();
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

        public AuthClient CreateClient(string name, AuthApplicationType applicationType, string allowedOrigin)
        {
            name = name.Trim();
            if(string.IsNullOrWhiteSpace(name))
                throw new ArgumentException(nameof(name), "client name cannot be empty");

            var origin = applicationType == AuthApplicationType.NativeConfidential ? "*" : allowedOrigin;
            if (string.IsNullOrWhiteSpace(origin))
                throw new ArgumentException("allowed origin is invalid or empty", nameof(allowedOrigin));

            if (_authDbContext.Clients.Any(p => p.Name == name))
                throw new ArgumentException(nameof(name), "client with requested name already exists");

            var appId = Guid.NewGuid().ToString("N");
            var appSecret = $"{Guid.NewGuid().ToString("N")}{Guid.NewGuid().ToString("N")}";

            var authClient = new AuthClient
            {
                Id = appId,
                Name = name.Trim(),
                ApplicationType = applicationType,
                AllowedOrigin = origin,
                CreatedAt = DateTime.Now,
                Active = true,
                RefreshTokenLifeTime = DefaultRefreshTokenExpiration[applicationType],
                Secret = GetHash(appSecret) // in db we save hashed client secret
            };

            _authDbContext.Clients.Add(authClient);
            _authDbContext.SaveChanges();

            // user gets real app secret
            authClient.Secret = appSecret;

            return authClient;
        }

        public IEnumerable<AuthClient> GetAllClients()
        {
            return _authDbContext.Clients.OrderByDescending(p => p.CreatedAt).ToList();
        }

        public async Task<bool> DeleteClient(string appId)
        {
            if (string.IsNullOrWhiteSpace(appId)) throw new ArgumentException(nameof(appId));

            var authClient = new AuthClient { Id = appId };
            _authDbContext.Entry(authClient).State = EntityState.Deleted;
            return (await _authDbContext.SaveChangesAsync()) > 0;
        }

        public async Task<bool> ToggleAppState(string appId, bool isActive)
        {
            if (string.IsNullOrWhiteSpace(appId)) throw new ArgumentException(nameof(appId));

            var authClient = await _authDbContext.Clients.FindAsync(appId);
            if (authClient == null) return false;
            if (authClient.Active == isActive) return true;

            authClient.Active = isActive;
            return (await _authDbContext.SaveChangesAsync()) > 0;
        }

        public async Task<string> ReGenerateAppSecret(string appId)
        {
            if (string.IsNullOrWhiteSpace(appId)) throw new ArgumentException(nameof(appId));

            var authClient = await _authDbContext.Clients.FindAsync(appId);
            if (authClient == null) throw new ArgumentException(nameof(appId), "application nof found");

            var newSecret = $"{Guid.NewGuid().ToString("N")}{Guid.NewGuid().ToString("N")}";
            authClient.Secret = GetHash(newSecret);
            await _authDbContext.SaveChangesAsync();

            return newSecret;
        }

        public AuthClient GenerateOfficialClientId()
        {
            if (_officialJsClient == null)
            {
                lock (Lock)
                {
                    if (_officialJsClient == null)
                    {
                        var officialClient = _authDbContext.Clients.FirstOrDefault(p => p.Name == OfficialAppName);
                        if (officialClient == null)
                        {
                            _officialJsClient = CreateClient(OfficialAppName, AuthApplicationType.JavaScript,
                                _environment.BaseAddress);
                        }
                        else
                        {
                            _officialJsClient = officialClient;
                            if (!officialClient.Active)
                            {
                                // activate official client if is was disabled
                                var result = ToggleAppState(officialClient.Id, true).Result;
                            }
                        }

                        return _officialJsClient;
                    }
                }
            }

            return _officialJsClient;
        }

        private static readonly Dictionary<AuthApplicationType, int> DefaultRefreshTokenExpiration = new Dictionary<AuthApplicationType, int>
        {
            {AuthApplicationType.JavaScript, 7200 },
            {AuthApplicationType.NativeConfidential, 14440 }
        };
    }
}

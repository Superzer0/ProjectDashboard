using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Common.Logging;
using Dashboard.Infrastructure.Identity.Managers;
using Dashboard.UI.Objects.Auth;

namespace Dashboard.Infrastructure.Identity.Repository
{
    public class AuthRepository
    {
        private readonly ILog _log = LogManager.GetLogger<AuthRepository>();
        private readonly ApplicationUserManager _userManager;

        public AuthRepository(ApplicationUserManager userManager)
        {
            _userManager = userManager;
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
    }
}

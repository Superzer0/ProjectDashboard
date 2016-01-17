using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Common.Logging;
using Dashboard.Models.Account;
using Dashboard.UI.Objects.Auth;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Dashboard.Infrastructure.Identity
{
    public class AuthRepository : IDisposable
    {
        private readonly ILog _log = LogManager.GetLogger<AuthRepository>();
        private readonly AuthContext _ctx;
        private readonly UserManager<DashboardUser> _userManager;

        public AuthRepository()
        {
            _ctx = new AuthContext();
            _userManager = new UserManager<DashboardUser>(new UserStore<DashboardUser>(_ctx));
        }

        public async Task<IdentityResult> RegisterUser(RegisterViewModel model)
        {
            var user = new DashboardUser { UserName = model.Email, Email = model.Email };
            var result = await _userManager.CreateAsync(user, model.Password);
            return result;
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

        public void Dispose()
        {
            _ctx.Dispose();
            _userManager.Dispose();
        }
    }
}

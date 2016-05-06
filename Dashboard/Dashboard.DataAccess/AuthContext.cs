using System.Data.Entity;
using Dashboard.UI.Objects.Auth;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Dashboard.DataAccess
{
    public class AuthDbContext : IdentityDbContext<DashboardUser>
    {
        public AuthDbContext()
            : base("EmbeddedUsersDbContext", throwIfV1Schema: false)
        {
            Database.SetInitializer<AuthDbContext>(null);
        }

        public DbSet<AuthClient> Clients { get; set; }
        public DbSet<AuthRefreshToken> RefreshTokens { get; set; }
    }
}

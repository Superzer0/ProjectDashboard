using System.Data.Entity;
using Dashboard.Models.Account;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Dashboard.Infrastructure.Identity
{
    public class AuthContext : IdentityDbContext<DashboardUser>
    {
        public AuthContext()
            : base("EmbeddedUsersDbContext", throwIfV1Schema: false)
        {
            Database.SetInitializer<AuthContext>(null);
        }

        public static AuthContext Create()
        {
            return new AuthContext();
        }
    }
}

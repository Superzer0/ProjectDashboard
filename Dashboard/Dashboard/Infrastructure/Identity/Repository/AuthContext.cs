using System.Data.Entity;
using Dashboard.UI.Objects.Auth;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Dashboard.Infrastructure.Identity.Repository
{
    public class AuthDbContext : IdentityDbContext<DashboardUser>
    {
        public AuthDbContext()
            : base("EmbeddedUsersDbContext", throwIfV1Schema: false)
        {
            Database.SetInitializer<AuthDbContext>(null);
        }
    }
}

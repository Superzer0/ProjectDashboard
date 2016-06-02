using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Dashboard.Infrastructure.Identity.Managers
{
    public class ApplicationRoleManager : RoleManager<IdentityRole>
    {
        // only for unit testing
        public ApplicationRoleManager() : base(new RoleStore<IdentityRole>())
        {
            
        }

        public ApplicationRoleManager(IRoleStore<IdentityRole, string> store) : base(store)
        {
        }
    }
}

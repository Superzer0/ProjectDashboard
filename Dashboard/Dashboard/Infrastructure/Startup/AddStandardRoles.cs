using System;
using System.Linq;
using Common.Logging;
using Dashboard.Infrastructure.Identity.Managers;
using Dashboard.UI.Objects;
using Dashboard.UI.Objects.Auth;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Dashboard.Infrastructure.Startup
{
    internal class AddStandardRoles : IExecuteAtStartup
    {
        private readonly ApplicationRoleManager _applicationRoleManager;
        private readonly ILog _logger = LogManager.GetLogger<AddStandardRoles>();

        public AddStandardRoles(Func<ApplicationRoleManager> aplicationManagerFactory)
        {
            _applicationRoleManager = aplicationManagerFactory();
        }

        public void Execute()
        {
            try
            {
                _logger.Info(m => m("Checking standard roles..."));
                
                if (_applicationRoleManager.Roles.Any()) return;

                _applicationRoleManager.Create(new IdentityRole(DashboardRoles.User));
                _applicationRoleManager.Create(new IdentityRole(DashboardRoles.Admin));
                _applicationRoleManager.Create(new IdentityRole(DashboardRoles.PluginManager));
                _logger.Info(m => m("Created standard roles"));
            }
            catch (Exception e)
            {
                _logger.Error(m => m("Error while creating standard roles"), e);
            }
        }
    }
}

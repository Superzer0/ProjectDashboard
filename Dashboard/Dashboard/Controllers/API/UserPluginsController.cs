using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using Dashboard.Infrastructure.Controllers;
using Dashboard.Infrastructure.Identity;
using Dashboard.UI.Objects.Providers;

namespace Dashboard.Controllers.API
{
    [RoutePrefix("api/user/plugins")]
    [Authorize(Roles = DashboardRoles.User)]
    public class UserPluginsController : BaseController
    {
        private readonly IProvidePlugins _providePlugins;

        public UserPluginsController(IProvidePlugins providePlugins)
        {
            _providePlugins = providePlugins;
        }

        [HttpGet]
        [Route("")]
        public async Task<IHttpActionResult> UserPlugins()
        {
            var plugins =
                (await _providePlugins.GetActivePluginsAsync()).Select(
                    p => new
                    {
                        p.Id,
                        p.Name,
                        p.Version,
                        p.Disabled,
                        icon = GetPluginIconUrl(p)
                    });

            return Ok(plugins);
        }

        [HttpGet]
        [Route("active")]
        public async Task<IHttpActionResult> ActiveUserPlugins()
        {
            var user = await GetCurrentUser();
            var plugins =
                (await _providePlugins.GetActiveUserPluginsAsync(user.Id))
                .Select(
                    p => new
                    {
                        p.Name,
                        p.Version,
                        icon = GetPluginIconUrl(p)
                    });

            return Ok(plugins);
        }

        [HttpGet]
        [Route("get/{pluginId:guid}/{version}")]
        [Authorize(Roles = DashboardRoles.PluginManager)]
        public async Task<IHttpActionResult> GetUserPluginConfiguration(string pluginId, string version)
        {
            var plugin = _providePlugins.GetPluginAsync(pluginId, version);
            if (plugin == null) return NotFound();

            var user = await GetCurrentUser();

            var userConfiguration = await _providePlugins.GetUserPluginConfiguration(pluginId, version, user.Id);

            return Ok(new
            {
                plugin.Id,
                plugin.Version,
                plugin.Name,
                disabled = userConfiguration?.Disabled ?? true,
                configuration = userConfiguration?.JsonConfiguration ?? plugin.Configuration,
                icon = GetPluginIconUrl(plugin)
            });
        }

        [HttpPost]
        [Route("change/configuration/{appId:guid}/{version}")]
        public async Task<IHttpActionResult> ChangeUserPluginConfiguration(Guid appId, string version)
        {
            return Ok();
        }

    }
}

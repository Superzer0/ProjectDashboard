using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using Dashboard.Infrastructure.Controllers;
using Dashboard.UI.Objects.Auth;
using Dashboard.UI.Objects.Providers;
using Newtonsoft.Json.Linq;
using static Dashboard.UI.Objects.Auth.DashboardRoles;

namespace Dashboard.Controllers.API
{
    [RoutePrefix("api/user/plugins")]
    [Authorize(Roles = DashboardRoles.User)]
    public class UserPluginsController : BaseController
    {
        private readonly IProvidePlugins _providePlugins;
        private readonly IManagePlugins _managePlugins;
        public UserPluginsController(IProvidePlugins providePlugins, IManagePlugins managePlugins)
        {
            _providePlugins = providePlugins;
            _managePlugins = managePlugins;
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
                        icon = p.IconUrl(Environment.PluginsPath)
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
                        p.Id,
                        p.Name,
                        p.Version,
                        icon = p.IconUrl(Environment.PluginsPath)
                    });

            return Ok(plugins);
        }

        [HttpGet]
        [Route("get/{pluginId:guid}/{version}")]
        [Authorize(Roles = PluginManager)]
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
                icon = plugin.IconUrl(Environment.PluginsPath)
            });
        }

        [HttpPost]
        [Route("change/configuration/{appId:guid}/{version}")]
        public IHttpActionResult ChangeUserPluginConfiguration(Guid appId, string version)
        {
            return Ok();
        }

        [HttpPost]
        [Authorize(Roles = DashboardRoles.User)]
        [Route("switch/{appId:guid}/{version}/{user:guid}")]
        public async Task<IHttpActionResult> ChangePluginConfiguration(string appId, string version, string user, [FromBody] bool state)
        {
            try
            {
                await _managePlugins.SwitchPluginUserState(appId, version, user, state);
                return Ok();
            }
            catch (ArgumentNullException)
            {
                return BadRequest("Plugin or user not found");
            }
        }

        [HttpPost]
        [Authorize(Roles = DashboardRoles.User)]
        [Route("configuration/{appId:guid}/{version}/{user:guid}")]
        public async Task<IHttpActionResult> ChangePluginConfiguration(string appId, string version, string user, [FromBody] JToken configuration)
        {
            try
            {
                var configurationString = configuration?.ToString();
                if (string.IsNullOrWhiteSpace(configurationString)) return BadRequest(nameof(configuration));

                await _managePlugins.ChangeUserPluginConfiguration(appId, version, user, configurationString);
                return Ok();
            }
            catch (ArgumentNullException)
            {
                return BadRequest("Plugin or user not found");
            }
        }

    }
}

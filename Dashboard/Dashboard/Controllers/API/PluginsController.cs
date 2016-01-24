using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using Dashboard.Infrastructure.Controllers;
using Dashboard.Infrastructure.Identity;
using Dashboard.UI.Objects.Providers;

namespace Dashboard.Controllers.API
{
    [RoutePrefix("api/instance/plugins")]
    [Authorize(Roles = DashboardRoles.PluginManager)]
    public class PluginsController : BaseController
    {
        private readonly IProvidePlugins _providePlugins;

        public PluginsController(IProvidePlugins providePlugins)
        {
            _providePlugins = providePlugins;
        }

        [HttpGet]
        [Route("")]
        public async Task<IHttpActionResult> InstancePlugins()
        {
            var plugins =
                (await _providePlugins.GetPluginsAsync()).Select(
                    p => new
                    {
                        p.Id,
                        p.Version,
                        p.Name,
                        added = p.Added.ToString("d"),
                        p.Disabled,
                        p.UncompressedSize,
                        icon = GetPluginIconUrl(p)
                    });

            return Ok(plugins);
        }

        [HttpGet]
        [Route("get/{appId:guid}/{version}")]
        public async Task<IHttpActionResult> GetPluginInfo(string appId, string version)
        {
            var plugin = _providePlugins.GetPluginAsync(appId, version);

            if (plugin == null) return NotFound();

            var user = await UserManager.FindByIdAsync(plugin.AddedBy);

            return Ok(new
            {
                plugin.Id,
                plugin.Version,
                plugin.Name,
                added = plugin.Added.ToString("g"),
                plugin.Disabled,
                plugin.UncompressedSize,
                addedBy = $"{user?.UserName}, {user?.Email}",
                plugin.CommunicationType,
                plugin.Xml,
                plugin.Configuration,
                plugin.StartingProgram,
                icon = GetPluginIconUrl(plugin),
                api = plugin.PluginMethods.Select(r => new { r.Name, r.InputType, r.OutputType }).ToArray()
            });
        }

        [HttpPost]
        [Route("change/configuration/{appId:guid}/{version}")]
        public async Task<IHttpActionResult> ChangePluginConfiguration(Guid appId, string version)
        {
            return Ok(); //TODO
        }
    }
}

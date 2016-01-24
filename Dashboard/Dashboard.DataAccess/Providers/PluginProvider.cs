using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Common.Logging;
using Dashboard.UI.Objects.DataObjects;
using Dashboard.UI.Objects.Providers;

namespace Dashboard.DataAccess.Providers
{
    public class PluginProvider : IProvidePlugins
    {
        private readonly PluginsContext _pluginsContext;
        private readonly ILog _logger = LogManager.GetLogger<PluginProvider>();

        public PluginProvider(PluginsContext pluginsContext)
        {
            _pluginsContext = pluginsContext;
        }

        public async Task AddNewPlugin(Plugin plugin, IEnumerable<PluginMethod> pluginMethods)
        {
            try
            {
                foreach (var pluginMethod in pluginMethods)
                {
                    pluginMethod.PluginId = plugin.Id;
                    pluginMethod.PluginVersion = pluginMethod.PluginVersion;
                    pluginMethod.Plugin = plugin;
                }

                _pluginsContext.Plugins.Add(plugin);
                _pluginsContext.PluginMethods.AddRange(pluginMethods);
                await _pluginsContext.SaveChangesAsync();
            }
            catch (Exception e)
            {
                _logger.Error(e);
                throw;
            }
        }

        public Plugin GetPluginAsync(string pluginId, string version)
        {
            return _pluginsContext.Plugins.Find(pluginId, version);
        }

        public async Task<IEnumerable<Plugin>> GetPluginsAsync()
        {
            _pluginsContext.Configuration.LazyLoadingEnabled = false;
            return await _pluginsContext.Plugins.ToListAsync();
        }
        public async Task<IEnumerable<Plugin>> GetActivePluginsAsync()
        {
            return await _pluginsContext.Plugins.Where(p => !p.Disabled).ToListAsync();
        }

        public async Task<IEnumerable<Plugin>> GetActiveUserPluginsAsync(string userId)
        {
            var userPlugins = await _pluginsContext.PluginUiConfigurations.Where(p => p.UserId == userId).ToListAsync();
            var allEnabledPlugins = await _pluginsContext.Plugins.Where(p => !p.Disabled).ToListAsync();
            return
                allEnabledPlugins.Where(p => userPlugins.Any(r => !r.Disabled && p.Id == r.Id && r.Version == p.Version))
                    .ToList();
        }

        public async Task<PluginUiConfiguration> GetUserPluginConfiguration(string pluginId, string version, string userId)
        {
            return await _pluginsContext.PluginUiConfigurations.FindAsync(pluginId, version, userId);
        }
    }
}

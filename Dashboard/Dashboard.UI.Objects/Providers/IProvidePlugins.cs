using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dashboard.UI.Objects.DataObjects;

namespace Dashboard.UI.Objects.Providers
{
    public interface IProvidePlugins
    {
        Task AddNewPlugin(Plugin plugin, IEnumerable<PluginMethod> pluginMethods);
        Plugin GetPluginAsync(string pluginId, string version);
        Task<IEnumerable<Plugin>> GetPluginsAsync();
        Task<IEnumerable<Plugin>> GetActivePluginsAsync();
        Task<IEnumerable<Plugin>> GetActiveUserPluginsAsync(string userId);
        Task<PluginUiConfiguration> GetUserPluginConfiguration(string pluginId, string version, string userId);
    }
}

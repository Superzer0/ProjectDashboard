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
        Task SavePluginInfo(Plugin plugin);
        Task SavePluginConfiguration(PluginUiConfiguration pluginConfiguration);
        Task AddPluginUiConfiguration(PluginUiConfiguration pluginUiConfiguration);
        IEnumerable<Plugin> GetPluginVersions(string pluginId);
        Task<IEnumerable<PluginUiConfiguration>> GetActiveUserPluginsConfiguration(string userId);
    }
}

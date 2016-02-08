using System;
using System.Threading.Tasks;
using Dashboard.UI.Objects.DataObjects;
using Dashboard.UI.Objects.Providers;

namespace Dashboard.Services.Plugins
{
    internal class PluginsManager : IManagePlugins
    {
        private readonly IProvidePlugins _providePlugins;

        public PluginsManager(IProvidePlugins providePlugins)
        {
            _providePlugins = providePlugins;
        }

        public async Task SwitchPluginInstanceState(string appId, string version, bool state)
        {
            var plugin = _providePlugins.GetPluginAsync(appId, version);
            if (plugin == null) throw new ArgumentNullException(nameof(appId));
            plugin.Disabled = state;
            await _providePlugins.SavePluginInfo(plugin);
        }

        public async Task SwitchPluginUserState(string appId, string version, string user, bool state)
        {
            var pluginConfiguration = await _providePlugins.GetUserPluginConfiguration(appId, version, user);
            if (pluginConfiguration == null)
            {
                await CreateNewUiPluginConfiguration(new PluginUiConfiguration
                {
                    Version = version,
                    Disabled = state,
                    UserId = user,
                    Id = appId
                });
            }
            else
            {
                pluginConfiguration.Disabled = state;
                await _providePlugins.SavePluginConfiguration(pluginConfiguration);
            }
        }

        public async Task ChangeUserPluginConfiguration(string appId, string version, string user, string configuration)
        {
            if (string.IsNullOrWhiteSpace(configuration)) throw new ArgumentException("must not be empty", nameof(configuration));

            var pluginConfiguration = await _providePlugins.GetUserPluginConfiguration(appId, version, user);
            if (pluginConfiguration == null)
            {
                await CreateNewUiPluginConfiguration(new PluginUiConfiguration
                {
                    Version = version,
                    UserId = user,
                    Disabled = true,
                    JsonConfiguration = configuration,
                    Id = appId
                });
            }
            else
            {
                pluginConfiguration.JsonConfiguration = configuration;
                await _providePlugins.SavePluginConfiguration(pluginConfiguration);
            }
        }

        private async Task CreateNewUiPluginConfiguration(PluginUiConfiguration pluginUiConfiguration)
        {
            var plugin = _providePlugins.GetPluginAsync(pluginUiConfiguration.Id, pluginUiConfiguration.Version);
            if (plugin == null) throw new ArgumentNullException(nameof(pluginUiConfiguration.Id));

            pluginUiConfiguration.JsonConfiguration = string.IsNullOrWhiteSpace(pluginUiConfiguration.JsonConfiguration)
                ? plugin.Configuration
                : pluginUiConfiguration.JsonConfiguration;

            await _providePlugins.AddPluginUiConfiguration(pluginUiConfiguration);
        }
    }
}

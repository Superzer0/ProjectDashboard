using System;
using System.Threading.Tasks;
using Dashboard.UI.Objects.DataObjects;
using Dashboard.UI.Objects.Providers;

namespace Dashboard.Services.Plugins
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="Dashboard.UI.Objects.Providers.IManagePlugins" />
    internal class PluginsManager : IManagePlugins
    {
        private readonly IProvidePlugins _providePlugins;

        public PluginsManager(IProvidePlugins providePlugins)
        {
            _providePlugins = providePlugins;
        }

        /// <summary>
        /// Switches the state of the plugin instance.
        /// </summary>
        /// <param name="appId">The application identifier.</param>
        /// <param name="version">The version.</param>
        /// <param name="state">if set to <c>true</c> [state].</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public async Task SwitchPluginInstanceState(string appId, string version, bool state)
        {
            var plugin = _providePlugins.GetPluginAsync(appId, version);
            if (plugin == null) throw new ArgumentNullException(nameof(appId));
            plugin.Disabled = state;
            await _providePlugins.SavePluginInfo(plugin);
        }

        /// <summary>
        /// Switches the state of the plugin user.
        /// </summary>
        /// <param name="appId">The application identifier.</param>
        /// <param name="version">The version.</param>
        /// <param name="user">The user.</param>
        /// <param name="state">if set to <c>true</c> [state].</param>
        /// <returns></returns>
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

        /// <summary>
        /// Changes the user plugin configuration.
        /// </summary>
        /// <param name="appId">The application identifier.</param>
        /// <param name="version">The version.</param>
        /// <param name="user">The user.</param>
        /// <param name="configuration">The configuration.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException">must not be empty</exception>
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

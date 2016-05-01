using System;
using System.Linq;
using System.Threading.Tasks;
using Dashboard.UI.Objects.BrokerIntegration;
using Dashboard.UI.Objects.DataObjects.Execution;
using Dashboard.UI.Objects.Providers;
using Dashboard.UI.Objects.Services;

namespace Dashboard.Services.Remote
{
    /// <summary>
    /// Dispatches remote calls to Dashboard Broker
    /// </summary>
    /// <seealso cref="Dashboard.UI.Objects.Services.ICallRemoteMethods" />
    internal class RemoteCallsDispatcher : ICallRemoteMethods
    {
        private readonly IManageBrokerFacade _brokerFacade;
        private readonly IProvidePlugins _providePlugins;

        public RemoteCallsDispatcher(IManageBrokerFacade brokerFacade, IProvidePlugins providePlugins)
        {
            _brokerFacade = brokerFacade;
            _providePlugins = providePlugins;
        }

        /// <summary>
        /// Calls the remote method.
        /// </summary>
        /// <param name="brokerExecutionInfo">The broker execution information.</param>
        /// <param name="userId">The user identifier.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException">plugin not active or not found
        /// or
        /// called method not found in plugin api</exception>
        public async Task<string> CallRemoteMethod(BrokerExecutionInfo brokerExecutionInfo, string userId)
        {
            var plugins = (await _providePlugins.GetActiveUserPluginsAsync(userId)).ToList();

            var calledPlugin =
                plugins.FirstOrDefault(
                    p => p.Id == brokerExecutionInfo.PluginId && p.Version == brokerExecutionInfo.Version);

            if (calledPlugin == null)
            {
                throw new ArgumentException("plugin not active or not found");
            }

            if (!calledPlugin.PluginMethods.Any(
                p => brokerExecutionInfo.MethodName.Equals(p.Name, StringComparison.OrdinalIgnoreCase)))
            {
                throw new ArgumentException("called method not found in plugin api");
            }

            if (string.IsNullOrWhiteSpace(brokerExecutionInfo.Configuration))
            {
                var userPluginConfiguration = await _providePlugins.GetUserPluginConfiguration(calledPlugin.Id,
                    calledPlugin.Version, userId);

                brokerExecutionInfo.Configuration = userPluginConfiguration.JsonConfiguration;
            }

            return _brokerFacade.ExecutePlugin(brokerExecutionInfo);
        }
    }
}

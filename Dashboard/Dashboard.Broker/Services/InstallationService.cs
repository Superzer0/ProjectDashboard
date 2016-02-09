using System;
using Common.Logging;
using Dashboard.Broker.DataAccess.Providers;
using Dashboard.Broker.Objects.DataObjects;
using Dashboard.Broker.Objects.DataObjects.DataContracts;
using Dashboard.Broker.Objects.Providers;
using Dashboard.Broker.ServiceContracts;

namespace Dashboard.Broker.Services
{
    public class InstallationService : IInstallPluginsService
    {
        private readonly ILog _logger = LogManager.GetLogger<InstallationService>();
        private readonly IProvidePlugins _providePlugins = new StandardPluginProvider();
        private readonly IBrokerEnvironment _brokerEnvironment = new BrokerEnvironment();
        private readonly IManageBrokerStorage _manageBrokerStorage;

        public InstallationService()
        {
            _manageBrokerStorage = new BrokerStorageProvider(_brokerEnvironment);
        }

        public InstallationResult InstallPlugin(ZippedPlugin plugin)
        {
            try
            {
                var brokerPlugin = new BrokerPlugin
                {
                    Id = plugin.PluginId,
                    Name = plugin.Name,
                    Version = plugin.Version,
                    CheckSum = plugin.CheckSum,
                    CommunicationType = plugin.CommunicationType,
                    StartingProgram = plugin.StartingProgram
                };

                brokerPlugin.ExecutablePath = _brokerEnvironment.GetPluginRelativeExecPath(brokerPlugin);

                var zipPath = _manageBrokerStorage.SaveZippedPlugin(plugin.Zip, brokerPlugin);

                _manageBrokerStorage.UnzipPlugin(zipPath,brokerPlugin);

                _providePlugins.AddPlugin(brokerPlugin);

                return new InstallationResult { Successful = true };
            }
            catch (Exception e)
            {
                _logger.Error(e);
                throw;
            }
        }
    }
}

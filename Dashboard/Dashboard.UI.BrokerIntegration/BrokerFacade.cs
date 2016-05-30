using System;
using System.IO;
using AutoMapper;
using Common.Logging;
using Dashboard.UI.BrokerIntegration.BrokerExecution;
using Dashboard.UI.BrokerIntegration.BrokerInstallation;
using Dashboard.UI.BrokerIntegration.BrokerInstance;
using Dashboard.UI.Objects.BrokerIntegration;
using Dashboard.UI.Objects.DataObjects;
using Dashboard.UI.Objects.DataObjects.Execution;
using Dashboard.UI.Objects.Services.Plugins.Install;

namespace Dashboard.UI.BrokerIntegration
{
    public class BrokerFacade : IManageBrokerFacade
    {
        private readonly ILog _logger = LogManager.GetLogger<BrokerFacade>();
        private readonly LaunchPluginsServiceClient _launchPluginsServiceClient;
        private readonly InstallPluginsServiceClient _installPluginsServiceClient;
        private readonly ManageBrokerServiceClient _manageBrokerServiceClient;
        private readonly IMapper _mapper;

        public BrokerFacade(ManageBrokerServiceClient brokerServiceClient,
            LaunchPluginsServiceClient launchPluginsServiceClient,
            InstallPluginsServiceClient installPluginsServiceClient, IMapper mapper)
        {
            _manageBrokerServiceClient = brokerServiceClient;
            _launchPluginsServiceClient = launchPluginsServiceClient;
            _installPluginsServiceClient = installPluginsServiceClient;
            _mapper = mapper;
        }

        public void SendNewPlugin(string filePath, PluginInformation infoAboutPlugin)
        {
            using (var fileStream = File.Open(filePath, FileMode.Open))
            {
                _installPluginsServiceClient.InstallPlugin(infoAboutPlugin.CheckSum, (CommunicationType)Enum.Parse(typeof(CommunicationType), infoAboutPlugin.CommunicationType.ToString()),
                infoAboutPlugin.Name, infoAboutPlugin.PluginId, infoAboutPlugin.StartingProgram, infoAboutPlugin.Version, fileStream);

                _logger.Info($"mock, {filePath} file sent to plugin broker");
            }
        }

        public BrokerStats GetBrokerInformation()
        {
            var brokerInformation = _manageBrokerServiceClient.GetBrokerInformation();
            var brokerStats = new BrokerStats();
            _mapper.Map(brokerInformation, brokerStats);
            brokerStats.EndpointAddress = _manageBrokerServiceClient.Endpoint.Address.Uri.ToString();
            return brokerStats;
        }

        public string ExecutePlugin(BrokerExecutionInfo executionInfo)
        {
            var pluginExecutionInfo = new PluginExecutionInfo();
            _mapper.Map(executionInfo, pluginExecutionInfo);
            return _launchPluginsServiceClient.Execute(pluginExecutionInfo);
        }
    }
}

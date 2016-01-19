using Common.Logging;
using Dashboard.UI.Objects.BrokerIntegration;
using Dashboard.UI.Objects.Services.Plugins.Install;

namespace Dashboard.UI.BrokerIntegration
{
    public class BrokerFacade : IManageBrokerFacade
    {
        private readonly ILog _logger = LogManager.GetLogger<BrokerFacade>();

        public void SendNewPlugin(string filePath, PluginInformation infoAboutPlugin)
        {
            _logger.Info($"mock, {filePath} file sent to plugin broker");
        }
    }
}

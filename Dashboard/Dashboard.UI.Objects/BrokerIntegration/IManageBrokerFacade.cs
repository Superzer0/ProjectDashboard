using Dashboard.UI.Objects.DataObjects;
using Dashboard.UI.Objects.DataObjects.Execution;
using Dashboard.UI.Objects.Services.Plugins.Install;

namespace Dashboard.UI.Objects.BrokerIntegration
{
    public interface IManageBrokerFacade
    {
        void SendNewPlugin(string filePath, PluginInformation infoAboutPlugin);
        BrokerStats GetBrokerInformation();
        string ExecutePlugin(BrokerExecutionInfo executionInfo);
    }
}

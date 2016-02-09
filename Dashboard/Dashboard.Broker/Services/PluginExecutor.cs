using Common.Logging;
using Dashboard.Broker.Objects.DataObjects.DataContracts;
using Dashboard.Broker.Objects.PluginManager;
using Dashboard.Broker.ProcessManagement;
using Dashboard.Broker.ServiceContracts;

namespace Dashboard.Broker.Services
{
    public class PluginExecutor : ILaunchPluginsService
    {
        private readonly ILog _logger = LogManager.GetLogger<PluginExecutor>();
        private readonly IManagePluginProcesses _managePluginProcesses = new PluginProcessManager();

        public string Execute(PluginExecutionInfo pluginExecutionInfo)
        {
            return _managePluginProcesses.Execute(pluginExecutionInfo);
        }
    }
}

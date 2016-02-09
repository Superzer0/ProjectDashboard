using Dashboard.Broker.Objects.DataObjects.DataContracts;

namespace Dashboard.Broker.Objects.PluginManager
{
    public interface IManagePluginProcesses
    {
        string Execute(PluginExecutionInfo executionInfo);
    }
}

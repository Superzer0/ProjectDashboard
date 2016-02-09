using Dashboard.Broker.Objects.DataObjects.DataContracts;

namespace Dashboard.Broker.Objects.PluginManager
{
    public interface IEncodeExecutionParameters
    {
        string Encode(PluginExecutionInfo pluginExecutionInfo);
    }
}

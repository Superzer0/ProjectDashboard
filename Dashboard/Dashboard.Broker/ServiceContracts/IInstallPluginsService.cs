using System.ServiceModel;
using Dashboard.Broker.Objects.DataObjects.DataContracts;

namespace Dashboard.Broker.ServiceContracts
{
    [ServiceContract]
    public interface IInstallPluginsService
    {
        [OperationContract]
        InstallationResult InstallPlugin(ZippedPlugin plugin);
    }
}

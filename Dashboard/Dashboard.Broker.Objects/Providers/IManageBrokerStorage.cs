using System.IO;
using System.Threading.Tasks;
using Dashboard.Broker.Objects.DataObjects;

namespace Dashboard.Broker.Objects.Providers
{
    public interface IManageBrokerStorage
    {
        string SaveZippedPlugin(Stream fileStream, BrokerPlugin brokerPlugin);
        void UnzipPlugin(string filePath, BrokerPlugin pluginInfo);
        Task CleanUpUploadDirectory();
    }
}

using System.Linq;
using Dashboard.Broker.DataAccess.Providers;
using Dashboard.Broker.Objects.DataObjects.DataContracts;
using Dashboard.Broker.Objects.Providers;
using Dashboard.Broker.ServiceContracts;

namespace Dashboard.Broker.Services
{
    public class BrokerManagementService : IManageBrokerService
    {
        private readonly IBrokerEnvironment _brokerEnvironment;
        private readonly IProvidePlugins _providePlugins;

        public BrokerManagementService()
        {
            _brokerEnvironment = new BrokerEnvironment();
            _providePlugins = new StandardPluginProvider();
        }

        public BrokerInformation GetBrokerInformation()
        {
            return new BrokerInformation
            {
               Version = _brokerEnvironment.AppVersion,
               ExecutionPath = _brokerEnvironment.AppRootPath,
               SystemInfo = _brokerEnvironment.SystemInfo,
               Uptime = _brokerEnvironment.Uptime,
               PluginsCount = _providePlugins.GetPlugins().Count()
            };
        }
    }
}

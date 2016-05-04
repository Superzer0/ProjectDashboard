using Autofac;
using Dashboard.UI.BrokerIntegration.BrokerExecution;
using Dashboard.UI.BrokerIntegration.BrokerInstallation;
using Dashboard.UI.BrokerIntegration.BrokerInstance;

namespace Dashboard.DI.CompositionRoot
{
    public class BrokerIntegrationHandler : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<ManageBrokerServiceClient>().InstancePerRequest();
            builder.RegisterType<LaunchPluginsServiceClient>().InstancePerRequest();
            builder.RegisterType<InstallPluginsServiceClient>().InstancePerRequest();
        }
    }
}

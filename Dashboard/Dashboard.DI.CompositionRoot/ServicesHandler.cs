using Autofac;
using Dashboard.DataAccess.Services;
using Dashboard.Services.Plugins;
using Dashboard.Services.Plugins.Extract;
using Dashboard.Services.Plugins.Validation;
using Dashboard.UI.Objects.Services;
using Dashboard.UI.Objects.Services.Plugins;
using Dashboard.UI.Objects.Services.Plugins.Extract;
using Dashboard.UI.Objects.Services.Plugins.Validation;

namespace Dashboard.DI.CompositionRoot
{
    public class ServicesHandler : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<ConfigurationProvider>().As<IConfigureDashboard>().InstancePerRequest();

            builder.RegisterType<StandardPluginValidationBuilder>().As<IBuildValidationResult>().InstancePerRequest();
            builder.RegisterType<StandardPluginFacade>().As<IManagePluginsFacade>().InstancePerRequest();
            builder.RegisterType<StandardPluginInfoBuilder>().As<IBuildPluginInfo>().InstancePerRequest();
        }
    }
}

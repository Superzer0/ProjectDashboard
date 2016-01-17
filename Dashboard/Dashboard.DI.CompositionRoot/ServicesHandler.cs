using Autofac;
using Dashboard.DataAccess.Services;
using Dashboard.UI.Objects.Services;

namespace Dashboard.DI.CompositionRoot
{
    public class ServicesHandler : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<ConfigurationProvider>().As<IConfigureDashboard>().InstancePerRequest();
        }
    }
}

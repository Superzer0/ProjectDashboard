using Autofac;
using Dashboard.DataAccess.Providers;
using Dashboard.UI.Objects.Providers;

namespace Dashboard.DI.CompositionRoot
{
    public class ProvidersHandler : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<PluginProvider>().As<IProvidePlugins>();
            builder.RegisterType<PluginStorageProvider>().As<IManagePluginsStorage>();
        }
    }
}

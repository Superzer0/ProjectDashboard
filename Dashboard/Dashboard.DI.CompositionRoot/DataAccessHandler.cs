using Autofac;
using Dashboard.DataAccess;

namespace Dashboard.DI.CompositionRoot
{
    public class DataAccessHandler : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<PluginsContext>().AsSelf().InstancePerRequest();
        }
    }
}

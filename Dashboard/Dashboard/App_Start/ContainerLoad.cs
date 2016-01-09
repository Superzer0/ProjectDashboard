using Autofac;
using Dashboard.Infrastructure.Razor;

namespace Dashboard
{
    public class ContainerLoad : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<RazorEngineViewsExec>().As<IExecuteRazorViews>().SingleInstance();
        }
    }
}

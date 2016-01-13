using System.Reflection;
using System.Web.Http;
using Autofac;
using Autofac.Integration.WebApi;
using Dashboard.DataAccess;
using Dashboard.DI.CompositionRoot;
using Dashboard.Infrastructure.Controllers;
using Dashboard.Infrastructure.Filters;
using Dashboard.Infrastructure.Razor;
using Module = Autofac.Module;

namespace Dashboard
{
    public class DiContainerLoad : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<RazorEngineViewsExec>().As<IExecuteRazorViews>().SingleInstance();
        }

        internal static IContainer CreateContainer(HttpConfiguration configuration)
        {
            var builder = new ContainerBuilder();
            builder.RegisterApiControllers(Assembly.GetExecutingAssembly())
                .AssignableTo<RazorController>().OnActivated(args =>
                {
                    ((RazorController)args.Instance).ExecuteRazorViews
                        = args.Context.Resolve<IExecuteRazorViews>();
                });

            builder.RegisterWebApiFilterProvider(configuration);
            builder.RegisterAssemblyModules(Assembly.GetExecutingAssembly());
            builder.RegisterAssemblyModules(Assembly.GetAssembly(typeof(ServicesHandler)));
            RegisterFilters(builder);
            return builder.Build();
        }

        private static void RegisterFilters(ContainerBuilder builder)
        {
            builder.Register(p => new DbSessionFilter(p.Resolve<PluginsContext>()))
                .AsWebApiActionFilterFor<ApiController>()
                .InstancePerRequest();
        }
    }
}

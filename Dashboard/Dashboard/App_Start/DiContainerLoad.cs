using System.Reflection;
using System.Web.Http;
using Autofac;
using Autofac.Integration.WebApi;
using Dashboard.Controllers.API;
using Dashboard.DataAccess;
using Dashboard.DI.CompositionRoot;
using Dashboard.Infrastructure.Controllers;
using Dashboard.Infrastructure.Filters;
using Dashboard.Infrastructure.Identity;
using Dashboard.Infrastructure.Razor;
using Dashboard.Infrastructure.Services.Abstract;
using Dashboard.UI.Objects.Services;
using Module = Autofac.Module;

namespace Dashboard
{
    public class DiContainerLoad : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<RazorEngineViewsExec>().As<IExecuteRazorViews>().SingleInstance();
            builder.RegisterType<OwinSelfHostEnvironment>().As<IEnvironment>().InstancePerRequest();
            builder.RegisterType<AuthRepository>().AsSelf().InstancePerRequest();
        }

        internal static IContainer CreateContainer(HttpConfiguration configuration)
        {
            var builder = new ContainerBuilder();

            builder.RegisterApiControllers(Assembly.GetExecutingAssembly())
                .OnActivating(args =>
                {
                    if (args.Instance is RazorController)
                    {
                        ((RazorController)args.Instance).ExecuteRazorViews
                       = args.Context.Resolve<IExecuteRazorViews>();
                    }

                    if (args.Instance is BaseController)
                    {
                        ((BaseController)args.Instance).Environment
                        = args.Context.Resolve<IEnvironment>();
                    }

                }).InstancePerRequest();

            builder.RegisterWebApiFilterProvider(configuration);
            builder.RegisterAssemblyModules(Assembly.GetExecutingAssembly());
            builder.RegisterAssemblyModules(Assembly.GetAssembly(typeof(ServicesHandler)));
            RegisterFilters(builder);
            return builder.Build();
        }

        private static void RegisterFilters(ContainerBuilder builder)
        {
            builder.Register(p => new DbLoggingFilter(p.Resolve<PluginsContext>()))
             .AsWebApiActionFilterFor<BaseController>()
             .InstancePerRequest();
            
            builder.Register(p => new DbSessionFilter(p.Resolve<PluginsContext>()))
                .AsWebApiActionFilterFor<PluginsController>()
                .InstancePerRequest();

            builder.Register(p => new ErrorHandlingFilter())
                .AsWebApiExceptionFilterFor<ApiController>()
                .InstancePerRequest();
        }
    }
}

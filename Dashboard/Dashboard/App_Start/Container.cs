using System.Reflection;
using System.Web.Http;
using Autofac;
using Autofac.Integration.WebApi;
using Dashboard.Controllers.API;
using Dashboard.DataAccess;
using Dashboard.DI.CompositionRoot;
using Dashboard.Infrastructure.Controllers;
using Dashboard.Infrastructure.Filters;
using Dashboard.Infrastructure.Identity.Managers;
using Dashboard.Infrastructure.Identity.Repository;
using Dashboard.Infrastructure.Razor;
using Dashboard.Infrastructure.Services;
using Dashboard.Infrastructure.Startup;
using Dashboard.UI.Objects;
using Dashboard.UI.Objects.Auth;
using Dashboard.UI.Objects.Providers;
using Dashboard.UI.Objects.Services;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.DataProtection;
using Owin;

namespace Dashboard
{
    internal class Container
    {
        internal static IContainer Create(HttpConfiguration configuration, IAppBuilder appBuilder)
        {
            var builder = new ContainerBuilder();

            builder.RegisterApiControllers(Assembly.GetExecutingAssembly())
                .OnActivating(args =>
                {
                    if (args.Instance is RazorController)
                    {
                        ((RazorController)args.Instance).ExecuteRazorViews = args.Context.Resolve<IExecuteRazorViews>();
                    }

                    if (args.Instance is BaseController)
                    {
                        ((BaseController)args.Instance).Environment = args.Context.Resolve<IEnvironment>();
                        ((BaseController)args.Instance).RoleManager = args.Context.Resolve<ApplicationRoleManager>();
                        ((BaseController)args.Instance).UserManager = args.Context.Resolve<ApplicationUserManager>();
                    }

                }).InstancePerRequest();

            builder.RegisterWebApiFilterProvider(configuration);
            builder.RegisterAssemblyModules(Assembly.GetExecutingAssembly());
            builder.RegisterAssemblyModules(Assembly.GetAssembly(typeof(ServicesHandler)));
            RegisterFilters(builder);
            RegisterStartup(builder);
            RegisterAuthProviders(builder, appBuilder);
            return builder.Build();
        }

        private static void RegisterFilters(ContainerBuilder builder)
        {
            builder.Register(p => new DbLoggingFilter(p.Resolve<PluginsContext>()))
             .AsWebApiActionFilterFor<BaseController>()
             .InstancePerRequest();

            builder.Register(p => new DbSessionFilter(p.Resolve<PluginsContext>()))
                .AsWebApiActionFilterFor<PluginInstallationController>()
                .InstancePerRequest();

            builder.Register(p => new ErrorHandlingFilter())
                .AsWebApiExceptionFilterFor<ApiController>()
                .InstancePerRequest();
        }

        private static void RegisterStartup(ContainerBuilder builder)
        {
            builder.RegisterType<CleanUpTempDirectory>().As<IExecuteAtStartup>().InstancePerDependency();
            builder.RegisterType<MediaStreamFileProvider>().As<IProvideFiles>().InstancePerDependency();
            builder.RegisterType<RazorEngineViewsExec>().As<IExecuteRazorViews>().SingleInstance();
            builder.RegisterType<OwinSelfHostEnvironment>().As<IEnvironment>().SingleInstance();
        }

        private static void RegisterAuthProviders(ContainerBuilder builder, IAppBuilder app)
        {
            builder.Register(c => new UserStore<DashboardUser>(c.Resolve<AuthDbContext>()))
                .AsSelf()
                .AsImplementedInterfaces()
                .InstancePerRequest();

            builder.Register(c => new RoleStore<IdentityRole>(c.Resolve<AuthDbContext>()))
                .AsSelf()
                .AsImplementedInterfaces()
                .InstancePerRequest();

            builder.RegisterType<AuthRepository>().AsSelf().InstancePerRequest();
            builder.RegisterType<AuthDbContext>().AsSelf().InstancePerRequest();
            builder.RegisterType<ApplicationUserManager>().AsSelf().InstancePerRequest();
            builder.RegisterType<ApplicationRoleManager>().AsSelf().InstancePerRequest();

            builder.Register(c =>
            {
                var provider = new DpapiDataProtectionProvider("ProjectDashboard");
                return provider;
            }
                ).As<IDataProtectionProvider>().InstancePerRequest();
        }
    }
}

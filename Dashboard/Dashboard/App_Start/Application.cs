using System.Reflection;
using System.Web.Http;
using Autofac;
using Autofac.Integration.WebApi;
using Dashboard;
using Dashboard.Infrastructure.Controllers;
using Dashboard.Infrastructure.Razor;
using Microsoft.Owin;
using Microsoft.Owin.FileSystems;
using Microsoft.Owin.StaticFiles;
using Owin;

[assembly: OwinStartup(typeof(Application))]
namespace Dashboard
{
    public class Application
    {
        // order of function calls is critical
        public void Configuration(IAppBuilder app)
        {
            var configuration = new HttpConfiguration();
            RouteInitialization.Register(configuration.Routes);

            var container = CreateContainer();
            configuration.DependencyResolver = new AutofacWebApiDependencyResolver(container);
            app.UseAutofacMiddleware(container);
            app.UseWebApi(configuration);
            app.UseFileServer(new FileServerOptions
               {
                   FileSystem = new PhysicalFileSystem(@"../"),
                   EnableDirectoryBrowsing = true
               });
            app.UseDefaultFiles();
            app.UseErrorPage();
        }

        private IContainer CreateContainer()
        {
            var builder = new ContainerBuilder();
            builder.RegisterApiControllers(Assembly.GetExecutingAssembly())
                .AssignableTo<RazorController>().OnActivated(args =>
                {
                    ((RazorController)args.Instance).ExecuteRazorViews
                        = args.Context.Resolve<IExecuteRazorViews>();
                });
            builder.RegisterAssemblyModules(Assembly.GetExecutingAssembly());
            return builder.Build();
        }
    }
}
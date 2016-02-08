using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Formatting;
using System.Web.Http;
using Autofac;
using Autofac.Integration.WebApi;
using Dashboard;
using Dashboard.Infrastructure.Middleware;
using Dashboard.UI.Objects;
using Microsoft.Owin;
using Microsoft.Owin.Cors;
using Microsoft.Owin.FileSystems;
using Microsoft.Owin.StaticFiles;
using Newtonsoft.Json.Serialization;
using Owin;

[assembly: OwinStartup(typeof(Application))]
namespace Dashboard
{
    public partial class Application
    {
        // order of function calls is critical
        public void Configuration(IAppBuilder app)
        {
            var configuration = new HttpConfiguration();
            app.Use<GlobalExceptionLoggerMiddleware>();
            RouteInitialization.Register(configuration.Routes);
            configuration.MapHttpAttributeRoutes();
            var container = DiContainerLoad.CreateContainer(configuration);
            configuration.DependencyResolver = new AutofacWebApiDependencyResolver(container);
            app.UseAutofacMiddleware(container);
            ConfigureAuth(app);
            app.UseCors(CorsOptions.AllowAll);
            app.UseWebApi(configuration);
            app.UseFileServer(new FileServerOptions
            {
                FileSystem = new PhysicalFileSystem(@"../"),
                EnableDirectoryBrowsing = true
            });
            app.UseDefaultFiles();
            app.UseErrorPage();
            AddStandardRoles();

            var jsonMediaTypeFormatter = configuration.Formatters.OfType<JsonMediaTypeFormatter>().First();
            jsonMediaTypeFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            configuration.EnsureInitialized();

            BootstrapStartup(container);
        }

        private static void BootstrapStartup(IContainer container)
        {
            var tasks = container.Resolve<IEnumerable<IExecuteAtStartup>>().ToList();
            tasks.ForEach(p=> p.Execute());
        }
    }
}
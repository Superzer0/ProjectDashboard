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
// ReSharper disable UnusedMember.Global

[assembly: OwinStartup(typeof(Application))]
namespace Dashboard
{
    public partial class Application
    {
        
        public void Configuration(IAppBuilder app)
        {   
            // order of function calls is critical
            var configuration = new HttpConfiguration();
            app.Use<GlobalExceptionLoggerMiddleware>();
            RouteInitialization.Register(configuration.Routes);
            configuration.MapHttpAttributeRoutes();
            var container = Container.Create(configuration);
            configuration.DependencyResolver = new AutofacWebApiDependencyResolver(container);
            app.UseAutofacMiddleware(container);
            ConfigureAuth(app);
            app.UseCors(CorsOptions.AllowAll);
            app.UseWebApi(configuration);
            app.UseFileServer(new FileServerOptions
            {
                FileSystem = new PhysicalFileSystem(@"../"),
                EnableDirectoryBrowsing = false
            });
            app.UseDefaultFiles();
            app.UseErrorPage();

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
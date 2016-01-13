using System.Web.Http;
using Autofac.Integration.WebApi;
using Dashboard;
using Microsoft.Owin;
using Microsoft.Owin.Cors;
using Microsoft.Owin.FileSystems;
using Microsoft.Owin.StaticFiles;
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
            RouteInitialization.Register(configuration.Routes);
            configuration.MapHttpAttributeRoutes();
            ConfigureAuth(app);
            var container = DiContainerLoad.CreateContainer(configuration);
            configuration.DependencyResolver = new AutofacWebApiDependencyResolver(container);
            app.UseAutofacMiddleware(container);
            app.UseCors(CorsOptions.AllowAll);
            app.UseWebApi(configuration);
            app.UseFileServer(new FileServerOptions
            {
                FileSystem = new PhysicalFileSystem(@"../"),
                EnableDirectoryBrowsing = true
            });
            app.UseDefaultFiles();
            app.UseErrorPage();
        }
    }
}
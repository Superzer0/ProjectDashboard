using System.Web.Http;

namespace Dashboard
{
    public static class RouteInitialization
    {
        public static void Register(HttpRouteCollection routes)
        {
            routes.MapHttpRoute(
                name: "PublicSide",
                routeTemplate: "",
                defaults: new
                {
                    controller = "Home"
                });

            routes.MapHttpRoute(
                name: "Configuration",
                routeTemplate: "Configure",
                defaults: new
                {
                    controller = "Configure"
                });

            routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional });

        }
    }
}
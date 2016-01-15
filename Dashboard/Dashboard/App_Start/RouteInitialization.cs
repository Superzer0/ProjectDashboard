using System.Web.Http;

namespace Dashboard
{
    public static class RouteInitialization
    {
        public static void Register(HttpRouteCollection routes)
        {
            routes.MapHttpRoute(
                    name: "IndexRoute",
                    routeTemplate: "",
                    defaults: new
                    {
                        controller = "Admin",
                        action = "Index"
                    });
        }
    }
}
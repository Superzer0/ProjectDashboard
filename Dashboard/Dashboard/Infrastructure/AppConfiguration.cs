using System.Configuration;

namespace Dashboard.Infrastructure
{
    public static class AppConfiguration
    {
        public static string LocalAddress => ConfigurationManager.AppSettings["serverUrl"] ?? "http://localhost:9095";
    }
}

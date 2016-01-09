using System.Configuration;

namespace Dashboard.Infrastructure
{
    public static class AppConfiguration
    {
        public static string LocalAddress
        {
            get { return ConfigurationManager.AppSettings["serverUrl"] ?? "http://localhost:8080"; }
        }

    }
}

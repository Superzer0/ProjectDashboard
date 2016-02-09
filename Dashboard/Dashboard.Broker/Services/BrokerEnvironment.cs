using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using Dashboard.Broker.Objects.DataObjects;
using Dashboard.Broker.Objects.Providers;
using Dashboard.Common.PluginSchema;

namespace Dashboard.Broker.Services
{
    internal class BrokerEnvironment : IBrokerEnvironment
    {
        private const char RootPathIndicator = '~';

        public string MapPath(string relativePath)
        {
            if (string.IsNullOrWhiteSpace(relativePath))
                throw new ArgumentException("parameter must not be empty", nameof(relativePath));

            if (!relativePath.StartsWith(RootPathIndicator.ToString()))
                throw new ArgumentException("path must start with " + RootPathIndicator, nameof(relativePath));


            var absolutePath = relativePath.TrimStart(RootPathIndicator)
                .TrimStart('/')
                .Replace("/", Path.DirectorySeparatorChar.ToString());
            return Path.Combine(AppRootPath, absolutePath);
        }

        public string AppRootPath
        {
            get
            {
                var execPathUri = Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetName().CodeBase);
                var execPath = new Uri(execPathUri).LocalPath;
                return string.Join(Path.DirectorySeparatorChar.ToString(),
                    execPath.Split(new[] { Path.DirectorySeparatorChar }, StringSplitOptions.RemoveEmptyEntries)
                        .Reverse()
                        .Skip(1)
                        .Reverse()
                    );
            }
        }

        public string BaseAddress => GetSetting("serverUrl") ?? "http://localhost:8732";

        public string AppVersion => Assembly.GetExecutingAssembly().GetName().Version.ToString();

        public string PluginsPath => GetSetting("pluginsPath") ?? RootPathIndicator + "/plugins";

        public string PluginsZippedPath => GetSetting("pluginsZippedPath") ?? RootPathIndicator + "/plugins/zipped";

        public TimeSpan Uptime => DateTime.Now - Process.GetCurrentProcess().StartTime;

        public string SystemInfo => $"{Environment.OSVersion} on {Environment.MachineName}";

        public string GetPluginRelativeExecPath(BrokerPlugin brokerPlugin)
        {
            return $"{PluginsPath}/{brokerPlugin.UrlName}/{PluginZipStructure.ExecutableFolder}";
        }

        public string GetPluginRelativeInstallationPath(BrokerPlugin brokerPlugin)
        {
            return $"{PluginsPath}/{brokerPlugin.UrlName}";
        }

        public string GetPluginRelativeZippedPath(BrokerPlugin brokerPlugin)
        {
            return $"{PluginsZippedPath}/{brokerPlugin.UrlName}";
        }

        private string GetSetting(string key)
        {
            return ConfigurationManager.AppSettings[key];
        }
    }
}

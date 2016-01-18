using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using Dashboard.UI.Objects.Services;

namespace Dashboard.Infrastructure.Services.Abstract
{
    public class OwinSelfHostEnvironment : IEnvironment
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

        public string BaseAddress => GetSetting("serverUrl") ?? "http://localhost:8080";

        public string AppVersion => Assembly.GetExecutingAssembly().GetName().Version.ToString();

        public string PluginsPath => GetSetting("pluginsPath") ?? (RootPathIndicator + "/plugins");

        public string PluginsUploadPath => GetSetting("uploadPluginsPath") ?? (RootPathIndicator + "/tempPlugins");

        private string GetSetting(string key)
        {
            return ConfigurationManager.AppSettings[key];
        }
    }
}

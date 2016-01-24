using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using Common.Logging;
using Dashboard.UI.Objects.DataObjects;
using Dashboard.UI.Objects.Providers;
using Dashboard.UI.Objects.Services;
using Dashboard.UI.Objects.Services.Plugins.Install;

namespace Dashboard.DataAccess.Providers
{
    internal class PluginStorageProvider : IManagePluginsStorage
    {
        private readonly IEnvironment _environment;
        private readonly ILog _logger = LogManager.GetLogger<PluginStorageProvider>();

        public PluginStorageProvider(IEnvironment environment)
        {
            _environment = environment;
        }

        public async Task UnzipPlugin(string filePath, PluginInformation pluginInfo)
        {
            await Task.Run(() =>
            {
                var dirName = GetPluginDirectoryName(pluginInfo);
                var destinationFolder = Path.Combine(_environment.MapPath(_environment.PluginsPath), dirName);

                ZipFile.ExtractToDirectory(filePath, destinationFolder);
                _logger.Info($"extracted {pluginInfo.Name} into {destinationFolder}");
            });
        }

        public async Task CleanUpUploadDirectory()
        {
            await Task.Run(() =>
            {

            });
        }

        private string GetPluginDirectoryName(PluginInformation pluginInfo)
        {
            return Plugin.GetUrlName(pluginInfo.Name, pluginInfo.PluginId, pluginInfo.Version);
        }
    }
}

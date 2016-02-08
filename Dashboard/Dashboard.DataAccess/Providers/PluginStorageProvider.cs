using System;
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
                var tempUploadPath = _environment.MapPath(_environment.PluginsUploadPath);
                _logger.Info($"cleaning up temp directory {tempUploadPath} ...");

                var di = new DirectoryInfo(tempUploadPath);
                foreach (var fileInfo in di.GetFiles())
                {
                    try
                    {
                        fileInfo.Delete();
                    }
                    catch (Exception)
                    {
                        _logger.Warn($"error while removing {fileInfo.Name}");
                    }
                }
                _logger.Info($"cleaned up temp directory {tempUploadPath}.");
            });
        }

        private string GetPluginDirectoryName(PluginInformation pluginInfo)
        {
            return Plugin.GetUrlName(pluginInfo.Name, pluginInfo.PluginId, pluginInfo.Version);
        }
    }
}

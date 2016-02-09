using System;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using Common.Logging;
using Dashboard.Common.PluginSchema;
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
                if (Directory.Exists(destinationFolder))
                {
                    _logger.Warn($"plugin  {pluginInfo.Name} already exists in {destinationFolder}");
                }
                else
                {
                    ZipFile.ExtractToDirectory(filePath, destinationFolder);
                    _logger.Info($"extracted {pluginInfo.Name} into {destinationFolder}");
                }
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

        public async Task<string> GetPluginIndexFile(Plugin pluginInfo)
        {
            var pluginFolder = Path.Combine(_environment.MapPath(_environment.PluginsPath), pluginInfo.UrlName);

            var indexFile = Path.Combine(pluginFolder,
                PluginZipStructure.PresentationEntryFile.Replace("/", @"\"));

            if (!File.Exists(indexFile)) return string.Empty;

            using (var streamReader = new StreamReader(File.OpenRead(indexFile)))
            {
                return await streamReader.ReadToEndAsync();
            }
        }

        private string GetPluginDirectoryName(PluginInformation pluginInfo)
        {
            return Plugin.GetUrlName(pluginInfo.Name, pluginInfo.PluginId, pluginInfo.Version);
        }
    }
}

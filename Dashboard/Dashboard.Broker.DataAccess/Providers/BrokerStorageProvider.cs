using System;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using Common.Logging;
using Dashboard.Broker.Objects.DataObjects;
using Dashboard.Broker.Objects.Providers;

namespace Dashboard.Broker.DataAccess.Providers
{
    internal class BrokerStorageProvider : IManageBrokerStorage
    {
        private const string PluginZipName = "plugin.zip";
        private readonly IBrokerEnvironment _environment;
        private readonly ILog _logger = LogManager.GetLogger<BrokerStorageProvider>();

        public BrokerStorageProvider(IBrokerEnvironment environment)
        {
            _environment = environment;
        }

        public string SaveZippedPlugin(Stream fileStream, BrokerPlugin brokerPlugin)
        {
            var destinationFolder = _environment.MapPath(_environment.GetPluginRelativeZippedPath(brokerPlugin));
            var destinationFile = Path.Combine(destinationFolder, PluginZipName);

            Directory.CreateDirectory(Path.GetDirectoryName(destinationFile));

            if (File.Exists(destinationFile))
            {
                _logger.Warn($"{destinationFile} already exists.");
            }
            else
            {
                using (var file = File.Create(destinationFile))
                {
                    fileStream.CopyTo(file);
                }
            }

            return destinationFile;
        }

        public void UnzipPlugin(string filePath, BrokerPlugin pluginInfo)
        {
            var destinationFolder = _environment.MapPath(_environment.GetPluginRelativeInstallationPath(pluginInfo));

            if (Directory.Exists(destinationFolder))
            {
                _logger.Info($"plugin {pluginInfo.Name} already exists in {destinationFolder}");
            }
            else
            {
                ZipFile.ExtractToDirectory(filePath, destinationFolder);
                _logger.Info($"extracted {pluginInfo.Name} into {destinationFolder}");
            }
        }

        public Task CleanUpUploadDirectory()
        {
            return Task.Run(() =>
            {
                var tempUploadPath = _environment.MapPath(_environment.PluginsZippedPath);
                _logger.Info($"cleaning up temp zipped plugins directory {tempUploadPath} ...");

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
    }
}

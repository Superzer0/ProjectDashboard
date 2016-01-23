using System.IO;
using Dashboard.Common.PluginSchema;
using Dashboard.UI.Objects.DataObjects;
using Dashboard.UI.Objects.DataObjects.Extract;
using Dashboard.UI.Objects.Services.Plugins.Extract;

namespace Dashboard.Services.Plugins.Extract.Builders
{
    internal class PluginJsonConfigurationExtactor : IExtractPluginInformation<PluginConfigurationInfo>
    {
        private readonly ZipHelper _zipHelper;

        public PluginJsonConfigurationExtactor(ZipHelper zipHelper)
        {
            _zipHelper = zipHelper;
        }

        public string Name => "PluginBasicZipInformationExtractor";

        public PluginConfigurationInfo Extract(ProcessedPlugin processedPlugin)
        {
            using (var zipArchive = _zipHelper.GetZipArchiveFromStream(processedPlugin.PluginZipStream))
            {
                var configurationEntry = _zipHelper.GetEntry(zipArchive, PluginZipStructure.ConfigurationFile);
                using (var streamReader = new StreamReader(configurationEntry.Open()))
                {
                    var configurationJson = streamReader.ReadToEnd();
                    return new PluginConfigurationInfo { IssuerName = Name, ConfigurationJson = configurationJson };
                }
            }
        }
    }
}

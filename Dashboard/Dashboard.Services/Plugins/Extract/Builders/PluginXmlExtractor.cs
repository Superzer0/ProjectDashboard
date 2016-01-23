using System.IO;
using Dashboard.Common.PluginSchema;
using Dashboard.Common.PluginXml;
using Dashboard.UI.Objects.DataObjects;
using Dashboard.UI.Objects.DataObjects.Extract;
using Dashboard.UI.Objects.Services.Plugins.Extract;

namespace Dashboard.Services.Plugins.Extract.Builders
{
    internal class PluginXmlExtractor : IExtractPluginInformation<PluginXmlInfo>
    {
        private readonly ZipHelper _zipHelper;

        public PluginXmlExtractor(ZipHelper zipHelper)
        {
            _zipHelper = zipHelper;
        }

        public string Name => "PluginBasicZipInformationExtractor";

        public PluginXmlInfo Extract(ProcessedPlugin processedPlugin)
        {
            using (var zipArchive = _zipHelper.GetZipArchiveFromStream(processedPlugin.PluginZipStream))
            {
                var configurationEntry = _zipHelper.GetEntry(zipArchive, PluginZipStructure.PluginXml);
                using (var streamReader = new StreamReader(configurationEntry.Open()))
                {
                    var rawXml = streamReader.ReadToEnd();
                    var deserializedPluginXml = PluginXml.Deserialize(rawXml);

                    return new PluginXmlInfo { IssuerName = Name, RawXml = rawXml, PluginXml = deserializedPluginXml };
                }
            }
        }
    }
}

using Dashboard.UI.Objects.DataObjects;
using Dashboard.UI.Objects.DataObjects.Extract;
using Dashboard.UI.Objects.Services.Plugins.Extract;

namespace Dashboard.Services.Plugins.Extract.Builders
{
    public class PluginXmlExtractor : IExtractPluginInformation<PluginXmlInfo>
    {
        public string Name => "PluginBasicZipInformationExtractor";

        public PluginXmlInfo Extract(ProcessedPlugin plugin)
        {
            return new PluginXmlInfo();
        }
    }
}

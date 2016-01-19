using Dashboard.UI.Objects.DataObjects;
using Dashboard.UI.Objects.DataObjects.Extract;
using Dashboard.UI.Objects.Services.Plugins.Extract;

namespace Dashboard.Services.Plugins.Extract.Builders
{
    public class PluginJsonConfigurationExtactor : IExtractPluginInformation<PluginConfigurationInfo>
    {
        public string Name => "PluginBasicZipInformationExtractor";

        public PluginConfigurationInfo Extract(ProcessedPlugin plugin)
        {
            return new PluginConfigurationInfo { IssuerName = Name };
        }
    }
}

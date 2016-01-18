using Dashboard.UI.Objects.DataObjects;
using Dashboard.UI.Objects.DataObjects.Extract;
using Dashboard.UI.Objects.Services.Plugins.Extract;

namespace Dashboard.Services.Plugins.Extract.Builders
{
    public class PluginBasicZipInformationExtractor : IExtractPluginInformation<PluginZipBasicInformation>
    {
        public string Name => "PluginBasicZipInformationExtractor";

        public PluginZipBasicInformation Extract(ProcessedPlugin plugin)
        {
            return new PluginZipBasicInformation();
        }
    }
}

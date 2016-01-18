using Dashboard.UI.Objects.DataObjects.Extract;

namespace Dashboard.UI.Objects.Services.Plugins.Install
{
    public class PluginInformation
    {
        public PluginXmlInfo XmlInfo { get; set; }
        public PluginZipBasicInformation ZipInfo { get; set; }
        public PluginConfigurationInfo ConfigInfo { get; set; }
    }
}

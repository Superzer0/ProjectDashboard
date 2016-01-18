using Dashboard.UI.Objects.DataObjects.Extract;
using Dashboard.UI.Objects.Services.Plugins.Extract.Visitors;
using Dashboard.UI.Objects.Services.Plugins.Install;

namespace Dashboard.Services.Plugins.Extract.Visitors
{
    internal class GatherPluginInformationVisitor : IProcessPluginInformationVisitor
    {
        private readonly PluginInformation _pluginInformation = new PluginInformation();

        public void Visit(PluginZipBasicInformation leaf)
        {
            _pluginInformation.ZipInfo = leaf;
        }

        public void Visit(PluginXmlInfo leaf)
        {
            _pluginInformation.XmlInfo = leaf;
        }

        public void Visit(PluginConfigurationInfo leaf)
        {
            _pluginInformation.ConfigInfo = leaf;
        }

        public PluginInformation Result => _pluginInformation;
    }
}

using Dashboard.UI.Objects.DataObjects.Extract;

namespace Dashboard.UI.Objects.Services.Plugins.Extract.Visitors
{
    public interface IProcessPluginInformationVisitor
    {
        void Visit(PluginZipBasicInformation leaf);
        void Visit(PluginXmlInfo leaf);
        void Visit(PluginConfigurationInfo leaf);
    }
}

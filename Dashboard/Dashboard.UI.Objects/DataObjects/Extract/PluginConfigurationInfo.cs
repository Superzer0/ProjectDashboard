using Dashboard.UI.Objects.Services.Plugins.Extract.Visitors;

namespace Dashboard.UI.Objects.DataObjects.Extract
{
    public class PluginConfigurationInfo : BasePluginInformation
    {
        public string ConfigurationJson { get; set; }

        public override void Accept(IProcessPluginInformationVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}

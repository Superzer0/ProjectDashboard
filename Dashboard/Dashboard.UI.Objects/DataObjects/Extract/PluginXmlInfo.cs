using Dashboard.Common.PluginXml;
using Dashboard.UI.Objects.Services.Plugins.Extract.Visitors;

namespace Dashboard.UI.Objects.DataObjects.Extract
{
    public class PluginXmlInfo : BasePluginInformation
    {
        public string RawXml { get; set; }

        public PluginXml PluginXml { get; set; }

        public override void Accept(IProcessPluginInformationVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}

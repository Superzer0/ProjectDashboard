using Dashboard.UI.Objects.Services.Plugins.Extract.Visitors;

namespace Dashboard.UI.Objects.DataObjects.Extract
{
    public class CheckSumPluginInformation : BasePluginInformation
    {
        public string CheckSum { get; set; }

        public override void Accept(IProcessPluginInformationVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}

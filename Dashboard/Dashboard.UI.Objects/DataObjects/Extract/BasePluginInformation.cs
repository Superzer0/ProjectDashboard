using Dashboard.UI.Objects.Services.Plugins.Extract.Visitors;

namespace Dashboard.UI.Objects.DataObjects.Extract
{
    public abstract class BasePluginInformation
    {
        public string IssuerName { get; set; }
        public abstract void Accept(IProcessPluginInformationVisitor visitor);
    }
}

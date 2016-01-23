using Dashboard.UI.Objects.Services.Plugins.Extract.Visitors;

namespace Dashboard.UI.Objects.DataObjects.Extract
{
    public class PluginZipBasicInformation : BasePluginInformation
    {
        public long ArchiveSize { get; set; }
        public int FilesCount { get; set; }
        public long UncompressedSize { get; set; }

        public override void Accept(IProcessPluginInformationVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}

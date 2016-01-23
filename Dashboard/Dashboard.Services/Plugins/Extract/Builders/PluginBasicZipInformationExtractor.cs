using System.IO.Compression;
using System.Linq;
using Dashboard.UI.Objects.DataObjects;
using Dashboard.UI.Objects.DataObjects.Extract;
using Dashboard.UI.Objects.Services.Plugins.Extract;

namespace Dashboard.Services.Plugins.Extract.Builders
{
    internal class PluginBasicZipInformationExtractor : IExtractPluginInformation<PluginZipBasicInformation>
    {
        private readonly ZipHelper _zipHelper;

        public PluginBasicZipInformationExtractor(ZipHelper zipHelper)
        {
            _zipHelper = zipHelper;
        }

        public string Name => "PluginBasicZipInformationExtractor";

        public PluginZipBasicInformation Extract(ProcessedPlugin processedPlugin)
        {
            using (var zipArchive = _zipHelper.GetZipArchiveFromStream(processedPlugin.PluginZipStream))
            {
                var zippedPluginInfo = new PluginZipBasicInformation
                {
                    IssuerName = Name,
                    ArchiveSize = zipArchive.Entries.Aggregate(0L, (p, r) => p + r.CompressedLength),
                    UncompressedSize = zipArchive.Entries.Aggregate(0L, (p, r) => p + r.Length),
                    FilesCount = zipArchive.Entries.Count
                };

                return zippedPluginInfo;
            }
        }
    }
}

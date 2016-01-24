using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using Dashboard.Common.PluginSchema;

namespace Dashboard.Services.Plugins
{
    internal class ZipHelper
    {
        private const string SchemaResourceLocation = "Dashboard.Common.PluginSchema.PluginXmlSchema.xml";

        public virtual bool EntryNonEmpty(ZipArchive zipArchive, string entryName, ICollection<string> validationResults)
        {
            var zipEntry = zipArchive.Entries.FirstOrDefault(p => p.FullName.Equals(entryName));
            if (zipEntry?.Length > 0) return true;

            validationResults.Add($"file {entryName} must not be empty.");
            return false;
        }

        public virtual ZipArchive GetZipArchiveFromStream(Stream stream)
        {
            return new ZipArchive(stream, ZipArchiveMode.Read, true);
        }

        public virtual  ZipArchiveEntry GetEntry(ZipArchive zipArchive, string path)
        {
            return zipArchive.Entries.First(p => path.Equals(p.FullName, StringComparison.OrdinalIgnoreCase));
        }

        public virtual string GetPluginXsdSchema()
        {
            var commonAssembly = Assembly.GetAssembly(typeof(PluginZipStructure));

            using (var manifestResourceStream = commonAssembly.GetManifestResourceStream(SchemaResourceLocation))
            {
                if (manifestResourceStream == null || !manifestResourceStream.CanRead) throw new InvalidOperationException($"cannot load xml schema ({SchemaResourceLocation})");
                using (var streamReader = new StreamReader(manifestResourceStream))
                {
                    return streamReader.ReadToEnd();
                }
            }
        }

    }
}

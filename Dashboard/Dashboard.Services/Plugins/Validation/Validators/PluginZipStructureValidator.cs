using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using Dashboard.Common.PluginSchema;
using Dashboard.UI.Objects.DataObjects;
using Dashboard.UI.Objects.DataObjects.Validation;

namespace Dashboard.Services.Plugins.Validation.Validators
{
    internal class PluginZipStructureValidator : BasePluginValidator
    {
        public override string Name => "PluginZipStructureValidator";

        public override PluginValidationResult Validate(ProcessedPlugin processedPlugin)
        {
            using (var zipArchive = new ZipArchive(processedPlugin.PluginZipStream, ZipArchiveMode.Read, true))
            {
                var requiredFilesExist = true;
                var validationResults = new List<string>();

                //check all required files
                foreach (var expectedFile in _expectedFiles)
                {
                    if (zipArchive.Entries.Any(p => expectedFile.Equals(p.FullName, StringComparison.OrdinalIgnoreCase)))
                        continue; // found required entry, ok continue

                    requiredFilesExist = false;
                    validationResults.Add($"entry {expectedFile} not found in zip archive");
                }

                //check pluginxml content length
                var pluginXmlFileSizeOk = CheckEntryNonEmpty(zipArchive, PluginZipStructure.PluginXml, validationResults);

                //check index.html content length
                var presentationEntryFileSizeOk = CheckEntryNonEmpty(zipArchive,
                    PluginZipStructure.PresentationEntryFile, validationResults);

                var validationResult = new PluginValidationResult
                {
                    IsSuccess = requiredFilesExist && pluginXmlFileSizeOk && presentationEntryFileSizeOk,
                    ValidationResults = validationResults,
                    ValidatorName = Name
                };

                return validationResult;
            }
        }

        private readonly List<string> _expectedFiles = new List<string>
        {
            PluginZipStructure.ExecutableFolder,
            PluginZipStructure.PresentationFolder,
            PluginZipStructure.PresentationEntryFile,
            PluginZipStructure.PluginXml,
            PluginZipStructure.ConfigurationFile
        };
    }
}

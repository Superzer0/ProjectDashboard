using System;
using System.Collections.Generic;
using System.Linq;
using Dashboard.Common.PluginSchema;
using Dashboard.UI.Objects.DataObjects;
using Dashboard.UI.Objects.DataObjects.Validation;
using Dashboard.UI.Objects.Services.Plugins.Validation;

namespace Dashboard.Services.Plugins.Validation.Validators
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="Dashboard.UI.Objects.Services.Plugins.Validation.IValidatePlugin" />
    internal class PluginZipStructureValidator : IValidatePlugin
    {
        private readonly ZipHelper _zipHelper;
        public string Name => "PluginZipStructureValidator";

        public PluginZipStructureValidator(ZipHelper zipHelper)
        {
            _zipHelper = zipHelper;
        }

        /// <summary>
        /// Validates the specified processed plugin.
        /// </summary>
        /// <param name="processedPlugin">The processed plugin.</param>
        /// <returns></returns>
        public PluginValidationResult Validate(ProcessedPlugin processedPlugin)
        {
            using (var zipArchive = _zipHelper.GetZipArchiveFromStream(processedPlugin.PluginZipStream))
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
                var pluginXmlFileSizeOk = _zipHelper.EntryNonEmpty(zipArchive, PluginZipStructure.PluginXml, validationResults);

                //check index.html content length
                var presentationEntryFileSizeOk = _zipHelper.EntryNonEmpty(zipArchive,
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

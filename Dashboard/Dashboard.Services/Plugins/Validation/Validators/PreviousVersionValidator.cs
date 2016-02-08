using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Dashboard.Common.PluginSchema;
using Dashboard.Common.PluginXml;
using Dashboard.UI.Objects.DataObjects;
using Dashboard.UI.Objects.DataObjects.Validation;
using Dashboard.UI.Objects.Providers;
using Dashboard.UI.Objects.Services.Plugins.Validation;
using static System.String;

namespace Dashboard.Services.Plugins.Validation.Validators
{
    internal class PreviousVersionValidator : IValidatePlugin
    {
        private readonly ZipHelper _zipHelper;
        private readonly IProvidePlugins _providePlugins;

        public PreviousVersionValidator(ZipHelper zipHelper, IProvidePlugins providePlugins)
        {
            _zipHelper = zipHelper;
            _providePlugins = providePlugins;
        }

        public string Name => "PreviousVersionValidator";
        public PluginValidationResult Validate(ProcessedPlugin processedPlugin)
        {
            using (var zipArchive = _zipHelper.GetZipArchiveFromStream(processedPlugin.PluginZipStream))
            {
                var validationResults = new List<string>();
                var result = new PluginValidationResult
                {
                    ValidatorName = Name,
                    ValidationResults = validationResults
                };

                if (!_zipHelper.EntryNonEmpty(zipArchive, PluginZipStructure.PluginXml, validationResults))
                {
                    return result;
                }

                var configurationEntry = _zipHelper.GetEntry(zipArchive, PluginZipStructure.PluginXml);
                using (var streamReader = new StreamReader(configurationEntry.Open()))
                {
                    var rawXml = streamReader.ReadToEnd();
                    var deserializedPluginXml = PluginXml.Deserialize(rawXml);

                    var versionAndIdValid = !(IsNullOrWhiteSpace(deserializedPluginXml.PluginId) ||
                                          IsNullOrWhiteSpace(deserializedPluginXml.Version));

                    if (versionAndIdValid)
                    {
                        var versions = _providePlugins.GetPluginVersions(deserializedPluginXml.PluginId);

                        if (versions == null || !versions.Any()) // no previous plugins
                        {
                            result.IsSuccess = true;
                        }
                        else
                        {
                            if (versions.Any(p => p.Version == deserializedPluginXml.Version))
                            {
                                validationResults.Add(
                                    $"Plugin {deserializedPluginXml.PluginId}_{deserializedPluginXml.Version} already exist");
                            }
                            else
                            {
                                if (
                                    versions.Any(
                                        p =>
                                            Compare(p.Version, deserializedPluginXml.Version,
                                                StringComparison.InvariantCultureIgnoreCase) >
                                            0))
                                {
                                    validationResults.Add(
                                        "Plugin version is lower than already existing one");
                                }
                                else
                                {
                                    result.IsSuccess = true;
                                }
                            }
                        }
                    }
                    else
                    {
                        validationResults.Add("Plugin ID and Plugin Version must not be empty");
                    }

                    return result;
                }
            }
        }
    }
}

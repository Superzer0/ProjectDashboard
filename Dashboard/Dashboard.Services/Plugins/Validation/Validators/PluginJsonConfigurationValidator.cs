using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using Dashboard.Common.PluginSchema;
using Dashboard.UI.Objects.DataObjects;
using Dashboard.UI.Objects.DataObjects.Validation;
using Newtonsoft.Json.Linq;

namespace Dashboard.Services.Plugins.Validation.Validators
{
    internal class PluginJsonConfigurationValidator : BasePluginValidator
    {
        public override string Name => "PluginJsonConfigurationValidator";

        public override PluginValidationResult Validate(ProcessedPlugin processedPlugin)
        {
            using (var zipArchive = new ZipArchive(processedPlugin.PluginZipStream, ZipArchiveMode.Read, true))
            {
                var validationResults = new List<string>();
                var jsonValid = false;

                var jsonFileNonEmpty = CheckEntryNonEmpty(zipArchive, PluginZipStructure.ConfigurationFile,
                    validationResults);

                if (jsonFileNonEmpty)
                {
                    var jsonFileZipEntry = GetEntry(zipArchive, PluginZipStructure.ConfigurationFile);
                    using (var streamReader = new StreamReader(jsonFileZipEntry.Open()))
                    {
                        var jsonConfiguration = streamReader.ReadToEnd();

                        try
                        {
                            var jObject = JObject.Parse(jsonConfiguration);
                            jsonValid = jObject != null;
                        }
                        catch
                        {
                            validationResults.Add("not valid json file");
                            jsonValid = false;
                        }
                    }
                }

                return new PluginValidationResult
                {
                    IsSuccess = jsonValid,
                    ValidationResults = validationResults,
                    ValidatorName = Name
                };
            }
        }
    }
}

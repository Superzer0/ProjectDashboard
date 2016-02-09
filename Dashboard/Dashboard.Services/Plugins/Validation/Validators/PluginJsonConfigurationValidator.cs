using System.Collections.Generic;
using System.IO;
using Dashboard.Common.PluginSchema;
using Dashboard.UI.Objects.DataObjects;
using Dashboard.UI.Objects.DataObjects.Validation;
using Dashboard.UI.Objects.Services.Plugins.Validation;
using Newtonsoft.Json.Linq;

namespace Dashboard.Services.Plugins.Validation.Validators
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="Dashboard.UI.Objects.Services.Plugins.Validation.IValidatePlugin" />
    internal class PluginJsonConfigurationValidator : IValidatePlugin
    {
        private readonly ZipHelper _zipHelper;

        public PluginJsonConfigurationValidator(ZipHelper zipHelper)
        {
            _zipHelper = zipHelper;
        }

        public string Name => "PluginJsonConfigurationValidator";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="processedPlugin"></param>
        /// <returns></returns>
        public PluginValidationResult Validate(ProcessedPlugin processedPlugin)
        {
            using (var zipArchive = _zipHelper.GetZipArchiveFromStream(processedPlugin.PluginZipStream))
            {
                var validationResults = new List<string>();
                bool jsonValid;

                var jsonFileNonEmpty = _zipHelper.EntryNonEmpty(zipArchive, PluginZipStructure.ConfigurationFile,
                    validationResults);

                if (!jsonFileNonEmpty)
                {
                    return new PluginValidationResult
                    {
                        IsSuccess = false,
                        ValidationResults = validationResults,
                        ValidatorName = Name
                    };
                }

                var jsonFileZipEntry = _zipHelper.GetEntry(zipArchive, PluginZipStructure.ConfigurationFile);
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

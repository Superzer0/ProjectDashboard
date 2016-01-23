using System.Collections.Generic;
using System.IO;
using Dashboard.Common.PluginSchema;
using Dashboard.Common.PluginXml;
using Dashboard.DataAccess;
using Dashboard.UI.Objects.DataObjects;
using Dashboard.UI.Objects.DataObjects.Validation;
using Dashboard.UI.Objects.Services.Plugins.Validation;

namespace Dashboard.Services.Plugins.Validation.Validators
{
    internal class PreviousVersionValidator : IValidatePlugin
    {
        private readonly ZipHelper _zipHelper;
        private readonly PluginsContext _pluginsContext;

        public PreviousVersionValidator(ZipHelper zipHelper, PluginsContext pluginsContext)
        {
            _zipHelper = zipHelper;
            _pluginsContext = pluginsContext;
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

                if (_zipHelper.CheckEntryNonEmpty(zipArchive, PluginZipStructure.PluginXml, validationResults))
                {
                    return result;
                }

                var configurationEntry = _zipHelper.GetEntry(zipArchive, PluginZipStructure.PluginXml);
                using (var streamReader = new StreamReader(configurationEntry.Open()))
                {
                    var rawXml = streamReader.ReadToEnd();
                    var deserializedPluginXml = PluginXml.Deserialize(rawXml);

                    //TODO: add checking with db _pluginsContext

                    result.IsSuccess = true;

                    return result;
                }
            }
        }
    }
}

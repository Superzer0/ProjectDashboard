using System.Collections.Generic;
using Dashboard.UI.Objects.DataObjects;
using Dashboard.UI.Objects.DataObjects.Validation;
using Dashboard.UI.Objects.Services.Plugins.Validation;

namespace Dashboard.Services.Plugins.Validation.Validators
{
    internal class PluginXmlValidator : IValidatePlugin
    {
        public string Name => "PluginXmlValidator";

        public PluginValidationResult Validate(ProcessedPlugin processedPlugin)
        {
            return new PluginValidationResult
            {
                IsSuccess = true,
                ValidationResults = new List<string> {"ok"},
                ValidatorName = Name
            };
        }
    }
}

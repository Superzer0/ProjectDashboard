using System.Collections.Generic;
using Dashboard.UI.Objects.DataObjects;
using Dashboard.UI.Objects.DataObjects.Validation;

namespace Dashboard.UI.Objects.Services.Plugins.Validation
{
    public interface IBuildValidationResult
    {
        bool AllowDuplicateValidators { get; set; }
        void ConfigureStandard();
        void ConfigureBuilder(IEnumerable<IValidatePlugin> validators);
        IEnumerable<PluginValidationResult> Validate(ProcessedPlugin plugin);
    }
}

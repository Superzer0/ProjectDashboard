using Dashboard.UI.Objects.DataObjects;
using Dashboard.UI.Objects.DataObjects.Validation;

namespace Dashboard.UI.Objects.Services.Plugins.Validation
{
    public interface IValidatePlugin
    {
        string Name { get; }
        PluginValidationResult Validate(ProcessedPlugin processedPlugin);
    }
}

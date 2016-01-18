using System.Collections.Generic;

namespace Dashboard.UI.Objects.DataObjects.Validation
{
    public class ConsolidatedPluginValidationResult
    {
        public bool IsValidated { get; set; }
        public IEnumerable<PluginValidationResult> PluginValidationResults { get; set; }
    }
}

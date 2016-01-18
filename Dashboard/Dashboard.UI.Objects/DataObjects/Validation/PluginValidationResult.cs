using System.Collections.Generic;

namespace Dashboard.UI.Objects.DataObjects.Validation
{
    public class PluginValidationResult
    {
        public string ValidatorName { get; set; }

        public bool IsSuccess { get; set; }

        public IEnumerable<string> ValidationResults { get; set; }
    }
}

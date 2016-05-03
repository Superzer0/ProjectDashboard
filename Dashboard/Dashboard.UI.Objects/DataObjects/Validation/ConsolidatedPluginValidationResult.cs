using System.Collections.Generic;
using System.Linq;

namespace Dashboard.UI.Objects.DataObjects.Validation
{
    public class ConsolidatedPluginValidationResult
    {
        public bool IsValidated { get; set; }
        public IEnumerable<PluginValidationResult> PluginValidationResults { get; set; }

        protected bool Equals(ConsolidatedPluginValidationResult other)
        {
            return IsValidated == other.IsValidated &&
                   (Equals(PluginValidationResults, other.PluginValidationResults) ||
                    (PluginValidationResults?.All(p => other.PluginValidationResults?.All(r => r.Equals(p)) ?? false) ?? false));
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ConsolidatedPluginValidationResult)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (IsValidated.GetHashCode() * 397) ^ (PluginValidationResults?.GetHashCode() ?? 0);
            }
        }
    }
}

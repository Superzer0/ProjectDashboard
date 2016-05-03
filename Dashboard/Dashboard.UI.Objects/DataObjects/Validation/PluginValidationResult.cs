using System.Collections.Generic;
using System.Linq;

namespace Dashboard.UI.Objects.DataObjects.Validation
{
    /// <summary>
    /// Contains information about plugin validation result. Validator name is given in the ValidatorName property.
    /// </summary>
    public class PluginValidationResult
    {
        /// <summary>
        /// Gets the name of the validator.
        /// </summary>
        /// <value>
        /// The name of the validator.
        /// </value>
        public string ValidatorName { get; internal set; }

        /// <summary>
        /// Gets a value indicating whether this validation was success.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is success; otherwise, <c>false</c>.
        /// </value>
        public bool IsSuccess { get; internal set; }

        /// <summary>
        /// Contains validation errors if IsSuccess is false
        /// </summary>
        /// <value>
        /// The validation results.
        /// </value>
        public IEnumerable<string> ValidationResults { get; internal set; }

        protected bool Equals(PluginValidationResult other)
        {
            return IsSuccess == other.IsSuccess
                   && string.Equals(ValidatorName, other.ValidatorName)
                   && (Equals(ValidationResults, other.ValidationResults) ||
                       (ValidationResults?.All(p => other.ValidationResults?.All(r => r.Equals(p)) ?? false) ?? false));
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((PluginValidationResult)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = IsSuccess.GetHashCode();
                hashCode = (hashCode * 397) ^ (ValidationResults?.GetHashCode() ?? 0);
                hashCode = (hashCode * 397) ^ (ValidatorName?.GetHashCode() ?? 0);
                return hashCode;
            }
        }
    }
}

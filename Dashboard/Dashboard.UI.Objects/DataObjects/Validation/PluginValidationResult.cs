using System.Collections.Generic;

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
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using Common.Logging;
using Dashboard.Services.Plugins.Validation.Validators;
using Dashboard.UI.Objects.DataObjects;
using Dashboard.UI.Objects.DataObjects.Validation;
using Dashboard.UI.Objects.Services.Plugins.Validation;

namespace Dashboard.Services.Plugins.Validation
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="Dashboard.Services.Plugins.BaseBuilder{Dashboard.UI.Objects.Services.Plugins.Validation.IValidatePlugin}" />
    /// <seealso cref="Dashboard.UI.Objects.Services.Plugins.Validation.IBuildValidationResult" />
    class StandardPluginValidationBuilder : BaseBuilder<IValidatePlugin>, IBuildValidationResult
    {
        private readonly ZipSizeValidator _zipSizeValidator;
        private readonly PluginZipStructureValidator _zipStructureValidator;
        private readonly PluginJsonConfigurationValidator _pluginJsonConfigurationValidator;
        private readonly PluginXmlValidator _xmlValidator;
        private readonly PreviousVersionValidator _previousVersionValidator;
        private readonly ILog _logger = LogManager.GetLogger<StandardPluginValidationBuilder>();
        private const string ValidatorExceptionStandardMessage = "Error occurred while validation. Contact system administrator";

        public new bool AllowDuplicateValidators
        {
            get { return base.AllowDuplicateValidators; }
            set { base.AllowDuplicateValidators = value; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StandardPluginValidationBuilder"/> class.
        /// </summary>
        /// <param name="zipSizeValidator">The zip size validator.</param>
        /// <param name="zipStructureValidator">The zip structure validator.</param>
        /// <param name="pluginJsonConfigurationValidator">The plugin json configuration validator.</param>
        /// <param name="xmlValidator">The XML validator.</param>
        /// <param name="previousVersionValidator">The previous version validator.</param>
        public StandardPluginValidationBuilder(ZipSizeValidator zipSizeValidator, PluginZipStructureValidator zipStructureValidator,
            PluginJsonConfigurationValidator pluginJsonConfigurationValidator, PluginXmlValidator xmlValidator, PreviousVersionValidator previousVersionValidator)
        {
            _zipSizeValidator = zipSizeValidator;
            _zipStructureValidator = zipStructureValidator;
            _pluginJsonConfigurationValidator = pluginJsonConfigurationValidator;
            _xmlValidator = xmlValidator;
            _previousVersionValidator = previousVersionValidator;
        }

        /// <summary>
        /// Configures the standard.
        /// </summary>
        public void ConfigureStandard()
        {
            AppendValidatorToList(_zipSizeValidator);
            AppendValidatorToList(_zipStructureValidator);
            AppendValidatorToList(_pluginJsonConfigurationValidator);
            AppendValidatorToList(_xmlValidator);
            AppendValidatorToList(_previousVersionValidator);
        }

        /// <summary>
        /// Configures the builder.
        /// </summary>
        /// <param name="validators">The validators.</param>
        public void ConfigureBuilder(IEnumerable<IValidatePlugin> validators)
        {
            foreach (var validator in validators)
            {
                AppendValidatorToList(validator);
            }
        }

        /// <summary>
        /// Validates the specified plugin.
        /// </summary>
        /// <param name="plugin">The plugin.</param>
        /// <returns></returns>
        /// <exception cref="System.InvalidOperationException">No validators were added. To add validators user Configure method</exception>
        public IEnumerable<PluginValidationResult> Validate(ProcessedPlugin plugin)
        {
            var validationResults = new List<PluginValidationResult>();

            if (!Actions.Any()) throw new InvalidOperationException("No validators were added. To add validators user Configure method");

            foreach (var validator in Actions)
            {
                try
                {
                    validationResults.Add(validator.Validate(plugin));
                    plugin.ResetState();
                }
                catch (Exception e)
                {
                    _logger.Warn($"exception when executing {validator.Name} validator", e);
                    validationResults.Add(new PluginValidationResult
                    {
                        IsSuccess = false,
                        ValidationResults = new List<string> { ValidatorExceptionStandardMessage },
                        ValidatorName = validator.Name
                    });
                }
            }

            return validationResults;
        }
    }
}

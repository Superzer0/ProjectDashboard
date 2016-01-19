﻿using System;
using System.Collections.Generic;
using System.Linq;
using Common.Logging;
using Dashboard.Services.Plugins.Validation.Validators;
using Dashboard.UI.Objects.DataObjects;
using Dashboard.UI.Objects.DataObjects.Validation;
using Dashboard.UI.Objects.Services.Plugins.Validation;

namespace Dashboard.Services.Plugins.Validation
{
    class StandardPluginValidationBuilder : BaseBuilder<IValidatePlugin>, IBuildValidationResult
    {
        private readonly ILog _logger = LogManager.GetLogger<StandardPluginValidationBuilder>();
        private const string ValidatorExceptionStandardMessage = "Error occurred while validation. Contact system administrator";

        public new bool AllowDuplicateValidators
        {
            get { return base.AllowDuplicateValidators; }
            set { base.AllowDuplicateValidators = value; }
        }

        public StandardPluginValidationBuilder(bool allowDuplicateValidators = false)
        {
            AllowDuplicateValidators = allowDuplicateValidators;
        }

        public void ConfigureStandard()
        {
            AppendValidatorToList(new PluginZipStructureValidator());
            AppendValidatorToList(new PluginJsonConfigurationValidator());
            AppendValidatorToList(new PluginXmlValidator());
        }

        public void ConfigureBuilder(IEnumerable<IValidatePlugin> validators)
        {
            foreach (var validator in validators)
            {
                AppendValidatorToList(validator);
            }
        }

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

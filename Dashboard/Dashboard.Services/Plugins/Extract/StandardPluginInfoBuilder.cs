using System;
using System.Collections.Generic;
using System.Linq;
using Common.Logging;
using Dashboard.Services.Plugins.Extract.Builders;
using Dashboard.Services.Plugins.Validation;
using Dashboard.UI.Objects.DataObjects;
using Dashboard.UI.Objects.DataObjects.Extract;
using Dashboard.UI.Objects.Services.Plugins.Extract;

namespace Dashboard.Services.Plugins.Extract
{
    class StandardPluginInfoBuilder : BaseBuilder<IExtractPluginInformation<BasePluginInformation>>, IBuildPluginInfo
    {
        private readonly ILog _logger = LogManager.GetLogger<StandardPluginValidationBuilder>();

        public new bool AllowDuplicateValidators
        {
            get { return base.AllowDuplicateValidators; }
            set { base.AllowDuplicateValidators = value; }
        }

        public void ConfigureStandard()
        {
            AppendValidatorToList(new PluginBasicZipInformationExtractor());
            AppendValidatorToList(new PluginXmlExtractor());
            AppendValidatorToList(new PluginJsonConfigurationExtactor());
        }

        public void ConfigureBuilder(IEnumerable<IExtractPluginInformation<BasePluginInformation>> builders)
        {
            foreach (var builder in builders)
            {
                AppendValidatorToList(builder);
            }
        }

        public IEnumerable<BasePluginInformation> Build(ProcessedPlugin plugin)
        {
            var buildersResults = new List<BasePluginInformation>();

            if (!Actions.Any()) throw new InvalidOperationException("No builders were added. To add builders user Configure method");

            foreach (var validator in Actions)
            {
                try
                {
                    buildersResults.Add(validator.Extract(plugin));
                    plugin.ResetState();
                }
                catch (Exception e)
                {
                    _logger.Warn($"exception when executing {validator.Name} builder", e);
                }
            }

            return buildersResults;
        }
    }
}

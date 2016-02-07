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
    internal class StandardPluginInfoBuilder : BaseBuilder<IExtractPluginInformation<BasePluginInformation>>, IBuildPluginInfo
    {
        private readonly PluginBasicZipInformationExtractor _zipInformationExtractor;
        private readonly PluginXmlExtractor _xmlExtractor;
        private readonly PluginJsonConfigurationExtactor _pluginJsonConfigurationExtactor;
        private readonly CheckSumExtractor _checkSumExtractor;
        private readonly ILog _logger = LogManager.GetLogger<StandardPluginValidationBuilder>();

        public StandardPluginInfoBuilder(PluginBasicZipInformationExtractor zipInformationExtractor,
            PluginXmlExtractor xmlExtractor, PluginJsonConfigurationExtactor pluginJsonConfigurationExtactor, CheckSumExtractor checkSumExtractor)
        {
            _zipInformationExtractor = zipInformationExtractor;
            _xmlExtractor = xmlExtractor;
            _pluginJsonConfigurationExtactor = pluginJsonConfigurationExtactor;
            _checkSumExtractor = checkSumExtractor;
        }

        public new bool AllowDuplicateValidators
        {
            get { return base.AllowDuplicateValidators; }
            set { base.AllowDuplicateValidators = value; }
        }

        public void ConfigureStandard()
        {
            AppendValidatorToList(_zipInformationExtractor);
            AppendValidatorToList(_xmlExtractor);
            AppendValidatorToList(_pluginJsonConfigurationExtactor);
            AppendValidatorToList(_checkSumExtractor);
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

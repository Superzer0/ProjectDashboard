using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Dashboard.UI.Objects.DataObjects;
using Dashboard.UI.Objects.DataObjects.Validation;
using Dashboard.UI.Objects.Services.Plugins;
using Dashboard.UI.Objects.Services.Plugins.Validation;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Dashboard.DataAccess;
using Dashboard.Services.Plugins.Extract.Visitors;
using Dashboard.Services.Plugins.Install.Visitors;
using Dashboard.UI.Objects.BrokerIntegration;
using Dashboard.UI.Objects.Services.Plugins.Extract;
using Dashboard.UI.Objects.Services.Plugins.Install;

namespace Dashboard.Services.Plugins
{
    class StandardPluginFacade : IManagePluginsFacade
    {
        private readonly IBuildValidationResult _validationResultBuilder;
        private readonly IBuildPluginInfo _pluginInfoBuilder;
        private readonly IManageBrokerFacade _brokerFacade;
        private readonly PluginsContext _pluginsContext;

        private static readonly ConcurrentDictionary<string, PluginInstallation> ValidationQueue
            = new ConcurrentDictionary<string, PluginInstallation>();

        public StandardPluginFacade(IBuildValidationResult validationResultBuilder, IBuildPluginInfo pluginInfoBuilder, 
            IManageBrokerFacade brokerFacade, PluginsContext pluginsContext)
        {
            _validationResultBuilder = validationResultBuilder;
            _pluginInfoBuilder = pluginInfoBuilder;
            _brokerFacade = brokerFacade;
            _pluginsContext = pluginsContext;
        }

        public bool AddToValidationQueue(string fileId, string filePath)
        {
            if (!Path.IsPathRooted(filePath)) throw new ArgumentException("Plugin path is not rooted", nameof(filePath));

            return ValidationQueue.TryAdd(fileId, new PluginInstallation
            {
                State = PluginInstallationState.Uploaded,
                FilePath = filePath
            });
        }

        public async Task<ConsolidatedPluginValidationResult> ValidatePluginAsync(string fileId)
        {
            PluginInstallation pluginInstallation;

            if (!ValidationQueue.TryGetValue(fileId, out pluginInstallation))
            {
                return new ConsolidatedPluginValidationResult
                {
                    IsValidated = false,
                    PluginValidationResults = new List<PluginValidationResult>
                    {
                        new PluginValidationResult
                        {
                            IsSuccess = false,
                            ValidationResults = new List<string> {"fileId is not valid"},
                            ValidatorName = "ValidationQueueValidator"
                        }
                    }
                };
            }

            using (var processedPlugin = await GetPluginStream(fileId, pluginInstallation.FilePath))
            {
                _validationResultBuilder.ConfigureStandard();
                var validationResults = _validationResultBuilder.Validate(processedPlugin);
                var isValid = validationResults.All(p => p.IsSuccess);

                pluginInstallation.State = isValid
                    ? PluginInstallationState.Validated
                    : PluginInstallationState.ValidationFailed;

                return new ConsolidatedPluginValidationResult
                {
                    IsValidated = isValid,
                    PluginValidationResults = validationResults
                };
            }
        }

        public async Task<PluginInformation> GetPluginInstallableInformationAsync(string fileId)
        {
            PluginInstallation pluginInstallation;

            if (!ValidationQueue.TryGetValue(fileId, out pluginInstallation)
                || pluginInstallation.State != PluginInstallationState.Validated)
            {
                return new PluginInformation();
            }

            using (var processedPlugin = await GetPluginStream(fileId, pluginInstallation.FilePath))
            {
                _pluginInfoBuilder.ConfigureStandard();
                var infoCollection =  _pluginInfoBuilder.Build(processedPlugin).ToList();
                var infoVisitor = new GatherPluginInformationVisitor();
                infoCollection.ForEach(l => l.Accept(infoVisitor));
                return infoVisitor.Result;
            }
        }

        public async Task InstallPluginAsync(string fileId)
        {
            PluginInstallation pluginInstallation;

            if (!ValidationQueue.TryGetValue(fileId, out pluginInstallation)
                || pluginInstallation.State != PluginInstallationState.Validated)
            {
                throw new InvalidOperationException($"Cannot install plugin {fileId} because it was not found in context");
            }

            using (var processedPlugin = await GetPluginStream(fileId, pluginInstallation.FilePath))
            {
                _pluginInfoBuilder.ConfigureStandard();
                var infoCollection = _pluginInfoBuilder.Build(processedPlugin).ToList();
                var persistenceVisitor = new PersistPluginInformationVisitor(_pluginsContext);
                infoCollection.ForEach(l => l.Accept(persistenceVisitor));

                var infoVisitor = new GatherPluginInformationVisitor();
                infoCollection.ForEach(l => l.Accept(infoVisitor));

                var infoAboutPlugin = infoVisitor.Result;

                _brokerFacade.SendNewPlugin(pluginInstallation.FilePath, infoAboutPlugin);
            }
        }

        private async Task<ProcessedPlugin> GetPluginStream(string fileId, string filePath)
        {
            var memoryStream = new MemoryStream();
            using (var fileStream = File.OpenRead(filePath))
            {
                await fileStream.CopyToAsync(memoryStream);
                var processedPlugin = new ProcessedPlugin
                {
                    FileId = fileId,
                    PluginZipStream = memoryStream
                };
                return processedPlugin;
            }
        }
    }
}

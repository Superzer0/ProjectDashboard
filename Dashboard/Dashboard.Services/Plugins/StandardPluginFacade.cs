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
using Dashboard.Services.Plugins.Extract.Visitors;
using Dashboard.Services.Plugins.Install.Visitors;
using Dashboard.UI.Objects.BrokerIntegration;
using Dashboard.UI.Objects.DataObjects.Install.AutoMapping;
using Dashboard.UI.Objects.Providers;
using Dashboard.UI.Objects.Services.Plugins.Extract;
using Dashboard.UI.Objects.Services.Plugins.Install;

namespace Dashboard.Services.Plugins
{
    internal class StandardPluginFacade : IManagePluginsFacade
    {
        private readonly IBuildValidationResult _validationResultBuilder;
        private readonly IBuildPluginInfo _pluginInfoBuilder;
        private readonly IManageBrokerFacade _brokerFacade;
        private readonly IProvidePlugins _pluginsProvider;
        private readonly IManagePluginsStorage _pluginsStorage;

        private static readonly ConcurrentDictionary<string, PluginInstallation> ValidationQueue
            = new ConcurrentDictionary<string, PluginInstallation>();

        public StandardPluginFacade(IBuildValidationResult validationResultBuilder, IBuildPluginInfo pluginInfoBuilder,
            IManageBrokerFacade brokerFacade, IProvidePlugins pluginsProvider, IManagePluginsStorage pluginsStorage)
        {
            _validationResultBuilder = validationResultBuilder;
            _pluginInfoBuilder = pluginInfoBuilder;
            _brokerFacade = brokerFacade;
            _pluginsProvider = pluginsProvider;
            _pluginsStorage = pluginsStorage;
        }

        public bool AddToValidationQueue(string fileId, string filePath, Guid userId)
        {
            if (!Path.IsPathRooted(filePath))
                throw new ArgumentException("Plugin path is not rooted", nameof(filePath));

            return ValidationQueue.TryAdd(fileId, new PluginInstallation
            {
                State = PluginInstallationState.Uploaded,
                FilePath = filePath,
                UserId = userId
            });
        }

        public async Task<ConsolidatedPluginValidationResult> ValidatePluginAsync(string fileId, Guid userId)
        {
            var pluginInstallation = GetInstallationContextFromQueue(fileId, userId);

            if (pluginInstallation == null)
            {
                return new ConsolidatedPluginValidationResult
                {
                    IsValidated = false,
                    PluginValidationResults = new List<PluginValidationResult>
                    {
                        new PluginValidationResult
                        {
                            IsSuccess = false,
                            ValidationResults = new List<string> {"fileId is not valid or user does not have permissions"},
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

        public async Task<PluginInformation> GetPluginInstallableInformationAsync(string fileId, Guid userId)
        {
            var pluginInstallation = GetInstallationContextFromQueue(fileId, userId);

            if (pluginInstallation == null
                || pluginInstallation.State != PluginInstallationState.Validated)
            {
                return null;
            }

            using (var processedPlugin = await GetPluginStream(fileId, pluginInstallation.FilePath))
            {
                _pluginInfoBuilder.ConfigureStandard();
                var infoCollection = _pluginInfoBuilder.Build(processedPlugin).ToList();
                var infoVisitor = new GatherPluginInformationVisitor();
                infoCollection.ForEach(l => l.Accept(infoVisitor));
                return infoVisitor.Result;
            }
        }

        public async Task InstallPluginAsync(string fileId, Guid userId)
        {
            var pluginInstallation = GetInstallationContextFromQueue(fileId, userId);

            if (pluginInstallation == null
                || pluginInstallation.State != PluginInstallationState.Validated)
            {
                throw new InvalidOperationException($"Cannot install plugin {fileId} because it was not found in context or user {userId} does not have permission");
            }

            using (var processedPlugin = await GetPluginStream(fileId, pluginInstallation.FilePath))
            {
                AutoMapperConfiguration.Configure();
                _pluginInfoBuilder.ConfigureStandard();
                var infoCollection = _pluginInfoBuilder.Build(processedPlugin).ToList();
                var persistenceVisitor = new CombinePluginInformationVisitor();
                infoCollection.ForEach(l => l.Accept(persistenceVisitor));

                var infoVisitor = new GatherPluginInformationVisitor();
                infoCollection.ForEach(l => l.Accept(infoVisitor));
                var infoAboutPlugin = infoVisitor.Result;

                var plugin = persistenceVisitor.Plugin;
                var pluginMethods = persistenceVisitor.PluginMethods;

                plugin.AddedBy = userId.ToString();

                await _pluginsProvider.AddNewPlugin(plugin, pluginMethods);

                await _pluginsStorage.UnzipPlugin(pluginInstallation.FilePath, infoAboutPlugin);

                _brokerFacade.SendNewPlugin(pluginInstallation.FilePath, infoAboutPlugin);
            }
        }

        private PluginInstallation GetInstallationContextFromQueue(string fileId, Guid userId)
        {
            PluginInstallation pluginInstallation;

            if (!ValidationQueue.TryGetValue(fileId, out pluginInstallation))
                return null;

            return pluginInstallation.UserId != userId ? null : pluginInstallation;
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

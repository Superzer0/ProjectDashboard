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
using AutoMapper;
using Dashboard.Services.Plugins.Extract.Visitors;
using Dashboard.Services.Plugins.Install.Visitors;
using Dashboard.UI.Objects.BrokerIntegration;
using Dashboard.UI.Objects.Providers;
using Dashboard.UI.Objects.Services.Plugins.Extract;
using Dashboard.UI.Objects.Services.Plugins.Install;

namespace Dashboard.Services.Plugins
{
    /// <summary>
    /// Plugin Installation Facade
    /// </summary>
    /// <seealso cref="Dashboard.UI.Objects.Services.Plugins.IManagePluginsFacade" />
    internal class StandardPluginFacade : IManagePluginsFacade
    {
        private readonly IBuildValidationResult _validationResultBuilder;
        private readonly IBuildPluginInfo _pluginInfoBuilder;
        private readonly IManageBrokerFacade _brokerFacade;
        private readonly IProvidePlugins _pluginsProvider;
        private readonly IManagePluginsStorage _pluginsStorage;
        private readonly IMapper _mapper;

        private static readonly ConcurrentDictionary<string, PluginInstallation> ValidationQueue
            = new ConcurrentDictionary<string, PluginInstallation>();

        public StandardPluginFacade(IBuildValidationResult validationResultBuilder, IBuildPluginInfo pluginInfoBuilder,
            IManageBrokerFacade brokerFacade, IProvidePlugins pluginsProvider, IManagePluginsStorage pluginsStorage, IMapper mapper)
        {
            _validationResultBuilder = validationResultBuilder;
            _pluginInfoBuilder = pluginInfoBuilder;
            _brokerFacade = brokerFacade;
            _pluginsProvider = pluginsProvider;
            _pluginsStorage = pluginsStorage;
            _mapper = mapper;
        }

        /// <summary>
        /// Adds to validation queue. File will be addedas see="PluginInstallationState.Uploaded"
        /// </summary>
        /// <param name="fileId">The file identifier.</param>
        /// <param name="filePath">The file path.</param>
        /// <param name="userId">The user identifier.</param>
        /// <returns>Information if file was added to installation queue</returns>
        /// <exception cref="System.ArgumentException">Plugin path is not rooted</exception>
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

        /// <summary>
        /// Validates the plugin asynchronously.
        /// 
        /// </summary>
        /// <param name="fileId">The file identifier.</param>
        /// <param name="userId">The user identifier.</param>
        /// <returns>ConsolidatedPluginValidationResult which includes consolidated informations from validators</returns>
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

        /// <summary>
        /// Gets the plugin installable information asynchronously.
        /// </summary>
        /// <param name="fileId">The file identifier.</param>
        /// <param name="userId">The user identifier.</param>
        /// <returns>PluginInformation that consolidates all relevant information about zipped plugin during installation process</returns>
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
                var infoVisitor = new GatherPluginInformationVisitor(_pluginsProvider);
                infoCollection.ForEach(l => l.Accept(infoVisitor));
                infoVisitor.CombineData();
                return infoVisitor.Result;
            }
        }

        /// <summary>
        /// Installs plugin asynchronously.
        /// Plugin is unzipped to both Dashboard UI location and Dashboard Broker location
        /// </summary>
        /// <param name="fileId">The file identifier.</param>
        /// <param name="userId">The user identifier.</param>
        /// <returns></returns>
        /// <exception cref="System.InvalidOperationException"></exception>
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
                _pluginInfoBuilder.ConfigureStandard();
                var infoCollection = _pluginInfoBuilder.Build(processedPlugin).ToList();
                var persistenceVisitor = new CombinePluginInformationVisitor(_mapper);
                infoCollection.ForEach(l => l.Accept(persistenceVisitor));

                var infoVisitor = new GatherPluginInformationVisitor(_pluginsProvider);
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

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Dashboard.UI.Objects.DataObjects;
using Dashboard.UI.Objects.DataObjects.Display;
using Dashboard.UI.Objects.Providers;
using Dashboard.UI.Objects.Services;
using HtmlAgilityPack;
using Newtonsoft.Json;

namespace Dashboard.Services.Display
{
    /// <summary>
    /// Appends meta information about execution environment to plugin html
    /// </summary>
    /// <seealso cref="IPreparePluginFrontEnd" />
    internal class PluginFrontPreprocessor : IPreparePluginFrontEnd
    {
        private readonly IProvidePlugins _providePlugins;
        private readonly IManagePluginsStorage _pluginsStorage;

        /// <summary>
        /// Initializes a new instance of the <see cref="PluginFrontPreprocessor"/> class.
        /// </summary>
        /// <param name="providePlugins">Plugins Persistence Instance</param>
        /// <param name="pluginsStorage">Plugins Storage Instance</param>
        public PluginFrontPreprocessor(IProvidePlugins providePlugins, IManagePluginsStorage pluginsStorage)
        {
            _providePlugins = providePlugins;
            _pluginsStorage = pluginsStorage;
        }

        /// <summary>
        /// Processes the plugin HTML.
        /// </summary>
        /// <param name="pluginId">The plugin identifier.</param>
        /// <param name="version">Plugin version.</param>
        /// <param name="processingOptions">The processing options.</param>
        /// <returns>ProcessedPluginHtml with modified inner html</returns>
        public async Task<ProcessedPluginHtml> ProcessPluginHtml(string pluginId, string version, HtmlProcessingOptions processingOptions)
        {
            var plugin = _providePlugins.GetPluginAsync(pluginId, version);
            return await ProcessPluginHtml(plugin, processingOptions);
        }

        /// <summary>
        /// Processes the active plugins HTML.
        /// </summary>
        /// <param name="user">The user identifier.</param>
        /// <param name="processingOptions">The processing options.</param>
        /// <returns>Collection of ProcessedPluginHtml with modified inner html</returns>
        public async Task<IEnumerable<ProcessedPluginHtml>> ProcessActivePluginsHtml(string user, HtmlProcessingOptions processingOptions)
        {
            var plugins = await _providePlugins.GetActiveUserPluginsAsync(user);
            var result = new List<ProcessedPluginHtml>();

            foreach (var plugin in plugins)
            {
                result.Add(await ProcessPluginHtml(plugin, processingOptions));
            }  

            return result;
        }

        public async Task<IEnumerable<ProcessedPluginConfiguration>> ProcessActivePluginsConfiguration(string userId)
        {
            var plugins = await _providePlugins.GetActiveUserPluginsConfiguration(userId);
            var result = new List<ProcessedPluginConfiguration>();

            foreach (var plugin in plugins)
            {
                var displaySettings = JsonConvert.DeserializeObject<PluginDisplaySettings>(plugin.JsonConfiguration);

                // normalize values
                displaySettings.Columns = displaySettings.Columns > 12 ? 12 : displaySettings.Columns;
                displaySettings.Order = displaySettings.Order < 0 ? 0 : displaySettings.Order;

                result.Add(new ProcessedPluginConfiguration
                {
                    DisplaySettings = displaySettings,
                    JsonConfiguration = plugin.JsonConfiguration,
                    PluginUniqueId = Plugin.GetUniqueName(plugin.Id, plugin.Version),
                    DispatchLink = $"api/dispatch/{plugin.Id}/{plugin.Version}/"
                });
            }

            return result;
        }

        public IEnumerable<IEnumerable<Tuple<ProcessedPluginHtml, ProcessedPluginConfiguration>>> PackPluginHtmlToGrid(IEnumerable<ProcessedPluginHtml> processedPlugins,
                IEnumerable<ProcessedPluginConfiguration> pluginConfiguration)
        {
            var rows = new List<List<Tuple<ProcessedPluginHtml, ProcessedPluginConfiguration>>>();

            var configurationGroups = from p in processedPlugins
                                      join c in pluginConfiguration on Plugin.GetUniqueName(p.Plugin.Id, p.Plugin.Version) equals
                                          c.PluginUniqueId
                                      select new Tuple<ProcessedPluginHtml, ProcessedPluginConfiguration>(p, c);

            foreach (var configurationGroup in configurationGroups.OrderBy(p => p.Item2.DisplaySettings.Order))
            {
                var freeSpotFound = false;
                foreach (var row in rows)
                {
                    var columnsSum = row.Sum(p => p.Item2.DisplaySettings.Columns);
                    if (columnsSum + configurationGroup.Item2.DisplaySettings.Columns > 12) continue;

                    // found free spot
                    row.Add(configurationGroup);
                    freeSpotFound = true;
                    break;
                }

                if (!freeSpotFound)
                {
                    rows.Add(new List<Tuple<ProcessedPluginHtml, ProcessedPluginConfiguration>> { configurationGroup });
                }

            }

            return rows;
        }

        private async Task<ProcessedPluginHtml> ProcessPluginHtml(Plugin plugin,
            HtmlProcessingOptions processingOptions)
        {
            var rawContent = await _pluginsStorage.GetPluginIndexFile(plugin);
            var linkPrefix = $"{processingOptions.BaseAddress}/plugins/{plugin.UrlName}/front";
            var processedContent = UpdateResourceLinks(rawContent, processingOptions.ResourcePrefixTag, linkPrefix);
            processedContent = AddApiId(processedContent, processingOptions.ApiAppIdTag, Plugin.GetUniqueName(plugin.Id, plugin.Version));

            return new ProcessedPluginHtml
            {
                Plugin = plugin,
                Content = processedContent
            };
        }

        private string AddApiId(string content, string apiAppIdTag, string apiAppId)
        {
            var textReader = new StringReader(content);
            var document = new HtmlDocument();
            document.Load(textReader);

            var apiTag = document.DocumentNode.SelectSingleNode($"//div[@{apiAppIdTag}]");

            if (apiTag == null) return content;

            var attr = apiTag.Attributes[apiAppIdTag];
            attr.Value = apiAppId;
            var stringWriter = new StringWriter();
            document.Save(stringWriter);
            return stringWriter.ToString();
        }

        private string UpdateResourceLinks(string content, string resourceTag, string prefix)
        {
            var textReader = new StringReader(content);
            var document = new HtmlDocument();
            document.Load(textReader);

            var scriptTags = document.DocumentNode.SelectNodes($"//script[@{resourceTag}]");
            if (scriptTags != null)
            {
                foreach (var scriptTag in scriptTags)
                {
                    var att = scriptTag.Attributes["src"];
                    att.Value = $"{prefix}/{att.Value}";
                }
            }

            var imgTags = document.DocumentNode.SelectNodes($"//img[@{resourceTag}]");
            if (imgTags != null)
            {
                foreach (var imgTag in imgTags)
                {
                    var att = imgTag.Attributes["src"];
                    att.Value = $"{prefix}/{att.Value}";
                }
            }

            var linkTags = document.DocumentNode.SelectNodes($"//link[@{resourceTag}]");

            if (linkTags != null)
            {
                foreach (var linkTag in linkTags)
                {
                    var att = linkTag.Attributes["href"];
                    att.Value = $"{prefix}/{att.Value}";
                }
            }

            var stringWriter = new StringWriter();
            document.Save(stringWriter);
            return stringWriter.ToString();
        }

    }
}

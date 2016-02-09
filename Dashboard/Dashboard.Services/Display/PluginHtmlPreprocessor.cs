using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Dashboard.UI.Objects.DataObjects;
using Dashboard.UI.Objects.DataObjects.Display;
using Dashboard.UI.Objects.Providers;
using Dashboard.UI.Objects.Services;
using HtmlAgilityPack;

namespace Dashboard.Services.Display
{
    internal class PluginHtmlPreprocessor : IPreparePluginHtml
    {
        private readonly IProvidePlugins _providePlugins;
        private readonly IManagePluginsStorage _pluginsStorage;


        public PluginHtmlPreprocessor(IProvidePlugins providePlugins, IManagePluginsStorage pluginsStorage)
        {
            _providePlugins = providePlugins;
            _pluginsStorage = pluginsStorage;
        }

        public async Task<ProcessedPluginHtml> ProcessPluginHtml(string pluginId, string version, HtmlProcessingOptions processingOptions)
        {
            var plugin = _providePlugins.GetPluginAsync(pluginId, version);
            return await ProcessPluginHtml(plugin, processingOptions);
        }

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

        private async Task<ProcessedPluginHtml> ProcessPluginHtml(Plugin plugin,
            HtmlProcessingOptions processingOptions)
        {
            var rawContent = await _pluginsStorage.GetPluginIndexFile(plugin);
            var linkPrefix = $"{processingOptions.BaseAddress}/plugins/{plugin.UrlName}";
            var dispatchLink = $"api/dispatch/{plugin.Id}/{plugin.Version}/";
            var processedContent = UpdateResourceLinks(rawContent, processingOptions.ResourcePrefixTag, linkPrefix);
            processedContent = AddApiLink(processedContent, processingOptions.BaseAddressTag, dispatchLink);

            return new ProcessedPluginHtml
            {
                Plugin = plugin,
                Content = processedContent
            };
        }

        private string AddApiLink(string content, string apiLinkTag, string apiLink)
        {
            var textReader = new StringReader(content);
            var document = new HtmlDocument();
            document.Load(textReader);

            var apiTag = document.DocumentNode.SelectSingleNode($"//div[@{apiLinkTag}]");

            if (apiTag == null) return content;

            var attr = apiTag.Attributes[apiLinkTag];
            attr.Value = apiLink;
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

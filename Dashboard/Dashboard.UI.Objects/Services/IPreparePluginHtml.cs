using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dashboard.UI.Objects.DataObjects.Display;

namespace Dashboard.UI.Objects.Services
{
    public interface IPreparePluginFrontEnd
    {
        Task<ProcessedPluginHtml> ProcessPluginHtml(string version, string pluginId, HtmlProcessingOptions processingOptions);
        Task<IEnumerable<ProcessedPluginHtml>> ProcessActivePluginsHtml(string user, HtmlProcessingOptions processingOptions);
        Task<IEnumerable<ProcessedPluginConfiguration>> ProcessActivePluginsConfiguration(string userId);

        IEnumerable<IEnumerable<Tuple<ProcessedPluginHtml, ProcessedPluginConfiguration>>> PackPluginHtmlToGrid(
            IEnumerable<ProcessedPluginHtml> processedPlugins,
            IEnumerable<ProcessedPluginConfiguration> pluginConfiguration);
    }
}

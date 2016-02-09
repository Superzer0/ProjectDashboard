using System.Collections.Generic;
using System.Threading.Tasks;
using Dashboard.UI.Objects.DataObjects.Display;

namespace Dashboard.UI.Objects.Services
{
    public interface IPreparePluginHtml
    {
        Task<ProcessedPluginHtml> ProcessPluginHtml(string version, string pluginId, HtmlProcessingOptions processingOptions);
        Task<IEnumerable<ProcessedPluginHtml>> ProcessActivePluginsHtml(string user, HtmlProcessingOptions processingOptions);
    }
}

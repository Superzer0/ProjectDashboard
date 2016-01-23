using System.Threading.Tasks;
using Dashboard.UI.Objects.DataObjects.Validation;
using Dashboard.UI.Objects.Services.Plugins.Install;

namespace Dashboard.UI.Objects.Services.Plugins
{
    public interface IManagePluginsFacade
    {
        bool AddToValidationQueue(string fileId, string filePath);
        Task<ConsolidatedPluginValidationResult> ValidatePluginAsync(string fileId);
        Task<PluginInformation> GetPluginInstallableInformationAsync(string fileId);
        Task InstallPluginAsync(string fileId);
    }
}

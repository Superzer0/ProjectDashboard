using System;
using System.Threading.Tasks;
using Dashboard.UI.Objects.DataObjects.Validation;
using Dashboard.UI.Objects.Services.Plugins.Install;

namespace Dashboard.UI.Objects.Services.Plugins
{
    public interface IManagePluginsFacade
    {
        bool AddToValidationQueue(string fileId, string filePath, Guid userId);
        Task<ConsolidatedPluginValidationResult> ValidatePluginAsync(string fileId, Guid user);
        Task<PluginInformation> GetPluginInstallableInformationAsync(string fileId, Guid userId);
        Task InstallPluginAsync(string fileId, Guid userId);
    }
}

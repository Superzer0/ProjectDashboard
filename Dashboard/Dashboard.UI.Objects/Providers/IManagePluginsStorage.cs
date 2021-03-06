﻿using System.Threading.Tasks;
using Dashboard.UI.Objects.DataObjects;
using Dashboard.UI.Objects.Services.Plugins.Install;

namespace Dashboard.UI.Objects.Providers
{
    public interface IManagePluginsStorage
    {
        Task UnzipPlugin(string filePath, PluginInformation plugin);
        Task CleanUpUploadDirectory();
        Task<string> GetPluginIndexFile(Plugin plugin);
    }
}

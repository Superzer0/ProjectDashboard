using Dashboard.UI.Objects;
using Dashboard.UI.Objects.Providers;

namespace Dashboard.Infrastructure.Startup
{
    internal class CleanUpTempDirectory : IExecuteAtStartup
    {
        private readonly IManagePluginsStorage _pluginsStorage;

        public CleanUpTempDirectory(IManagePluginsStorage pluginsStorage)
        {
            _pluginsStorage = pluginsStorage;
        }

        public void Execute()
        {
            _pluginsStorage.CleanUpUploadDirectory();
        }
    }
}

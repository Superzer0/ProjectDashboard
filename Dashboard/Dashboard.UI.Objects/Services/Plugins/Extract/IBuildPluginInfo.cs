using System.Collections.Generic;
using Dashboard.UI.Objects.DataObjects;
using Dashboard.UI.Objects.DataObjects.Extract;

namespace Dashboard.UI.Objects.Services.Plugins.Extract
{
    public interface IBuildPluginInfo
    {
        bool AllowDuplicateValidators { get; set; }
        void ConfigureStandard();
        void ConfigureBuilder(IEnumerable<IExtractPluginInformation<BasePluginInformation>> builders);
        IEnumerable<BasePluginInformation> Build(ProcessedPlugin plugin);
    }
}

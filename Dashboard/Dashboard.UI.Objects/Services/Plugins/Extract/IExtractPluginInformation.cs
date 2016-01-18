using Dashboard.UI.Objects.DataObjects;
using Dashboard.UI.Objects.DataObjects.Extract;

namespace Dashboard.UI.Objects.Services.Plugins.Extract
{
    public interface IExtractPluginInformation<out TE> where TE : BasePluginInformation
    {
        string Name { get; }
        TE Extract(ProcessedPlugin plugin);
    }
}

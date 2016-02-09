namespace Dashboard.UI.Objects.Services
{
    public interface IEnvironment
    {
        string AppRootPath { get; }
        string MapPath(string relativePath);
        string BaseAddress { get; }
        string AppVersion { get; }
        string PluginsPath { get; }
        string PluginsUploadPath { get; }
        string EndpointAddress { get; }
    }
}

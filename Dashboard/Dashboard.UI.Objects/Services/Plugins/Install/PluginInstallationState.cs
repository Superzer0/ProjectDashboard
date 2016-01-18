namespace Dashboard.UI.Objects.Services.Plugins.Install
{
    public class PluginInstallation
    {
        public string FilePath { get; set; }
        public PluginInstallationState State { get; set; }
    }

    public enum PluginInstallationState
    {
        ValidationFailed,
        Uploaded,
        Validated,
        Installed,
        InstallationFailed
    }
}

using Dashboard.UI.Objects.DataObjects.Extract;

namespace Dashboard.UI.Objects.Services.Plugins.Install
{
    public class PluginInformation
    {
        public string Name { get; set; }
        public string PluginId { get; set; }
        public string Version { get; set; }
        public string CommunicationType { get; set; }
        public string StartingProgram { get; set; }
        public int MethodsCount { get; set; }

        public string ConfigurationJson { get; set; }
        public string RawXml { get; set; }

        public PluginZipBasicInformation ZipInfo { get; set; }
    }
}

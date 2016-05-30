namespace Dashboard.UI.Objects.DataObjects.Display
{
    public class ProcessedPluginConfiguration
    {
        public string PluginUniqueId { get; set; }
        public string JsonConfiguration { get; set; }
        public string DispatchLink { get; set; }
        public PluginDisplaySettings DisplaySettings { get; set; }
    }
}

using System.ComponentModel;
using Newtonsoft.Json;

namespace Dashboard.UI.Objects.DataObjects.Display
{
    public class PluginDisplaySettings
    {
        [DefaultValue(6)]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        public int Columns { get; set; }

        [DefaultValue(100)]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        public int Order { get; set; }
    }
}

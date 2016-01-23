using System.Xml.Serialization;

namespace Dashboard.Common.PluginXml
{
    public class PluginApi
    {
        [XmlArray("api")]
        [XmlArrayItem("method")]
        public PluginMethod[] Methods { get; set; }
    }
}

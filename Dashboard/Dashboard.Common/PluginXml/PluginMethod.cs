using System.Xml.Serialization;

namespace Dashboard.Common.PluginXml
{
    public class PluginMethod
    {
        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlElement("inputType")]
        public string InputType { get; set; }

        [XmlElement("outputType")]
        public string OutputType { get; set; }
    }
}

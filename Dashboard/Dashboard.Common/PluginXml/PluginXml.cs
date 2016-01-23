using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace Dashboard.Common.PluginXml
{
    [XmlRoot("plugin", Namespace = "", IsNullable = false)]
    public class PluginXml
    {
        [XmlElement("name")]
        public string Name { get; set; }

        [XmlElement("pluginId")]
        public string PluginId { get; set; }

        [XmlElement("version")]
        public string Version { get; set; }

        [XmlElement("communicationType")]
        public string CommunicationType { get; set; }

        [XmlElement("startingProgram")]
        public string StartingProgram { get; set; }

        [XmlElement("api")]
        public PluginApi Api { get; set; }

        public static PluginXml Deserialize(string inputXml)
        {
            var reader = XmlReader.Create(new StringReader(inputXml),
                new XmlReaderSettings { ConformanceLevel = ConformanceLevel.Document });
            return new XmlSerializer(typeof(PluginXml)).Deserialize(reader) as PluginXml;
        }
    }
}

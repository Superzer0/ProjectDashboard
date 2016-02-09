using System.IO;
using System.ServiceModel;
using Dashboard.Common;

namespace Dashboard.Broker.Objects.DataObjects.DataContracts
{
    [MessageContract]
    public class ZippedPlugin
    {
        [MessageHeader]
        public string PluginId { get; set; }

        [MessageHeader]
        public string Version { get; set; }

        [MessageHeader]
        public string Name { get; set; }

        [MessageHeader]
        public CommunicationType CommunicationType { get; set; }

        [MessageHeader]
        public string StartingProgram { get; set; }

        [MessageHeader]
        public string CheckSum { get; set; }

        [MessageBodyMember]
        public Stream Zip { get; set; }
    }
}

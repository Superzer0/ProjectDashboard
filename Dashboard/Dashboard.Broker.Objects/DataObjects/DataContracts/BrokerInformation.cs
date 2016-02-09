using System;
using System.Runtime.Serialization;

namespace Dashboard.Broker.Objects.DataObjects.DataContracts
{
    [DataContract]
    public class BrokerInformation
    {
        [DataMember]
        public string Version { get; set; }

        [DataMember]
        public TimeSpan Uptime { get; set; }

        [DataMember]
        public int PluginsCount { get; set; }

        [DataMember]
        public string SystemInfo { get; set; }

        [DataMember]
        public string ExecutionPath { get; set; }
    }
}

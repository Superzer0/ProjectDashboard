using System.Runtime.Serialization;

namespace Dashboard.Broker.Objects.DataObjects.DataContracts
{
    [DataContract]
    public class PluginExecutionInfo
    {
        [DataMember]
        public string PluginId { get; set; }

        [DataMember]
        public string Version { get; set; }

        [DataMember]
        public string Configuration { get; set; }

        [DataMember]
        public string MethodName { get; set; }

        [DataMember]
        public string Parameters { get; set; }

        public override string ToString()
        {
            return $"PluginId: {PluginId}, Version: {Version}, Configuration: {Configuration}, MethodName: {MethodName}, Parameters: {Parameters}";
        }
    }
}

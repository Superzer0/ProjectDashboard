using System.ServiceModel;

namespace Dashboard.Broker.Objects.DataObjects.DataContracts
{
    [MessageContract]
    public class InstallationResult
    {
        [MessageBodyMember]
        public bool Successful { get; set; }
    }
}

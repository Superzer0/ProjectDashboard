using System.Threading.Tasks;
using Dashboard.UI.Objects.DataObjects.Execution;

namespace Dashboard.UI.Objects.Services
{
    public interface ICallRemoteMethods
    {
        Task<string> CallRemoteMethod(BrokerExecutionInfo brokerExecutionInfo, string userId);
    }
}

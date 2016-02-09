using System;
using System.Text;
using Dashboard.Broker.Objects.DataObjects.DataContracts;
using Dashboard.Broker.Objects.PluginManager;

namespace Dashboard.Broker.ProcessManagement.ParametersEncoding.Encoders
{
    internal class Base64StandardEncoder : IEncodeExecutionParameters
    {
        public string Encode(PluginExecutionInfo pluginExecutionInfo)
        {
            var encodedMethodName = Convert.ToBase64String(Encoding.UTF8.GetBytes(pluginExecutionInfo.MethodName));
            var encodedParams = Convert.ToBase64String(Encoding.UTF8.GetBytes(pluginExecutionInfo.Parameters));

            return $"{encodedMethodName}.{encodedParams}";
        }
    }
}

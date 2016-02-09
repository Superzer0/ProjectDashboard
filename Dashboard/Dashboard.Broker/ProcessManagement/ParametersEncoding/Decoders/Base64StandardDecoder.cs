using System;
using System.Text;
using Dashboard.Broker.Objects.PluginManager;

namespace Dashboard.Broker.ProcessManagement.ParametersEncoding.Decoders
{
    internal class Base64StandardDecoder : IDecodeProcessOutput
    {
        public string Decode(string input)
        {
            return Encoding.UTF8.GetString(Convert.FromBase64String(input ?? string.Empty));
        }
    }
}

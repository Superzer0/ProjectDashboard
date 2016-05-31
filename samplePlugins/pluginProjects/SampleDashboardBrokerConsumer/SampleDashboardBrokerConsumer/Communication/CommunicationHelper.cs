using System;
using System.Text;
using log4net;
using Newtonsoft.Json;
using static System.StringSplitOptions;

namespace SampleDashboardBrokerConsumer.Communication
{
    public static class CommunicationHelper
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(CommunicationHelper));
        public static MethodCallInfo ReceiveMethodCall()
        {
            try
            {
                var line = Console.ReadLine() ?? string.Empty;
                var functionParams = line.Split(new[] { '.' }, RemoveEmptyEntries);

                var methodName = Encoding.UTF8.GetString(Convert.FromBase64String(functionParams[0] ?? string.Empty));
                var methodParams = string.Empty;

                if (functionParams.Length > 1)
                {
                    methodParams = Encoding.UTF8.GetString(Convert.FromBase64String(functionParams[1] ?? string.Empty));
                }

                return new MethodCallInfo { MethodName = methodName, CallParameters = methodParams };
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return null;
            }
        }

        public static Configuration GetConfiguration(string configuration)
        {
            if (string.IsNullOrWhiteSpace(configuration)) // sample build
                return new Configuration { JenkinsAddress = "https://builds.apache.org/view/Tika" };

            var plainText = Encoding.UTF8.GetString(Convert.FromBase64String(configuration));
            return JsonConvert.DeserializeObject<Configuration>(plainText);
        }

        public static void WriteResponse(object response)
        {
            var serializedObject = JsonConvert.SerializeObject(response);
            var payload = Convert.ToBase64String(Encoding.UTF8.GetBytes(serializedObject));
            Logger.Info($"serializedObject {serializedObject}");
            Logger.Info($"payload {payload}");
            Console.WriteLine(payload);
        }
    }
}

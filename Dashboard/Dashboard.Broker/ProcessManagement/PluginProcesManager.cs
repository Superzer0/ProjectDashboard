using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Common.Logging;
using Dashboard.Broker.DataAccess.Providers;
using Dashboard.Broker.Objects.DataObjects.DataContracts;
using Dashboard.Broker.Objects.PluginManager;
using Dashboard.Broker.Objects.Providers;
using Dashboard.Broker.ProcessManagement.ParametersEncoding.Decoders;
using Dashboard.Broker.ProcessManagement.ParametersEncoding.Encoders;
using Dashboard.Broker.Services;

namespace Dashboard.Broker.ProcessManagement
{
    internal class PluginProcessManager : IManagePluginProcesses
    {
        private readonly ILog _logger = LogManager.GetLogger<PluginProcessManager>();
        private readonly IProvidePlugins _providePlugins = new StandardPluginProvider();
        private readonly IDecodeProcessOutput _decodeProcessOutput = new Base64StandardDecoder();
        private readonly IEncodeExecutionParameters _encodeExecutionParameters = new Base64StandardEncoder();
        private readonly IBrokerEnvironment _brokerEnvironment = new BrokerEnvironment();

        private static readonly ConcurrentDictionary<string, Process> RunningProcessMap =
            new ConcurrentDictionary<string, Process>();

        public string Execute(PluginExecutionInfo executionInfo)
        {
            var plugin = _providePlugins.GetPlugin(executionInfo.PluginId, executionInfo.Version);

            if (plugin == null)
                throw new ArgumentNullException(nameof(executionInfo.PluginId), "plugin missing or not installed");

            var callCheckSum = GetConfigurationCheckSum(executionInfo);

            Process process;
            if (!RunningProcessMap.TryGetValue(callCheckSum, out process) || process.HasExited)
            {
                if (process != null)
                {
                    RunningProcessMap.TryRemove(callCheckSum, out process);
                }

                process = ConfigureNewProcess(plugin.ExecutablePath, plugin.StartingProgram, executionInfo.Configuration);
                RunningProcessMap.TryAdd(callCheckSum, process);
                process.Start();
                _logger.Info($"started plugin {plugin.Name} with params {executionInfo.Configuration} ({callCheckSum})");
            }

            var executionParams = _encodeExecutionParameters.Encode(executionInfo);

            try
            {
                _logger.Info($"executing plugin {plugin.Name} ({callCheckSum}) with params {executionInfo.Parameters}");
                process.StandardInput.WriteLine(executionParams);
                return _decodeProcessOutput.Decode(process.StandardOutput.ReadLine());
            }
            catch
            {
                if (process.HasExited == false)
                    process.Kill();

                RunningProcessMap.TryRemove(callCheckSum, out process);
                _logger.Error($"erro with ${plugin.UrlName}, checksum {callCheckSum}. Process killed");
                throw;
            }
        }

        private Process ConfigureNewProcess(string fileLocation, string startingProgram, string configuration)
        {
            var startingFile = Path.Combine(_brokerEnvironment.MapPath(fileLocation), startingProgram);

            var jobProcess = new Process
            {
                StartInfo =
                {
                    WindowStyle = ProcessWindowStyle.Hidden,
                    FileName = startingFile,
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    Arguments = Convert.ToBase64String(Encoding.UTF8.GetBytes(configuration))
                }
            };

            return jobProcess;
        }

        private string GetConfigurationCheckSum(PluginExecutionInfo executionInfo)
        {
            using (var cryptoProvider = new SHA1CryptoServiceProvider())
            {
                var identifier = $"{executionInfo.PluginId}.{executionInfo.Version}.{executionInfo.Configuration}";
                return BitConverter.ToString(cryptoProvider.ComputeHash(Encoding.UTF8.GetBytes(identifier)));
            }
        }
    }
}

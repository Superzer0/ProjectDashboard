using System.Collections.Generic;
using System.Linq;
using Jenkins.Domain;
using SampleDashboardBrokerConsumer.Communication;
using ILog = log4net.ILog;
using LogManager = log4net.LogManager;

namespace SampleDashboardBrokerConsumer
{
    class Program
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(Program));
        private static Configuration _configuration;
        static void Main(string[] args)
        {
            _configuration = CommunicationHelper.GetConfiguration(args.FirstOrDefault());
            Logger.Info($"received configuration {_configuration.JenkinsAddress}");
            Logger.Info("jenkins server instantiated");

            /*            For DEBUG
             *            Jenkins.InitServer(new Configuration { JenkinsAddress = "http://localhost:8080/view/sample" });
                        var jobs = Jenkins.GetJobs();
                        var jenkinsJobs = jobs.Select(p => new
                        {
                            name = p.Name,
                            description = p.Description,
                            isInQueue = p.IsInQueue,
                            state = p.Status?.State,
                            lastBuildDuration = p.LastBuild?.Duration,
                            lastSuccessDate = p.LastSuccessfulBuild?.TimeStamp,
                            lastFaildate = p.LastFailedBuild?.TimeStamp
                        }).ToList();*/

            do
            {
                var methodInfo = CommunicationHelper.ReceiveMethodCall();
                Logger.Info($"received method {methodInfo.MethodName}");
                var response = DispathMethodCall(methodInfo);
                //Debugger.Launch(); way to debug
                CommunicationHelper.WriteResponse(
                    response.Select(p => new
                    {
                        name = p.Name,
                        description = p.Description,
                        isInQueue = p.LastBuild?.IsBuilding ?? false,
                        state = p.Status?.State,
                        lastBuildDuration = p.LastBuild?.Duration?.TotalMinutes,
                        lastSuccessDate = p.LastSuccessfulBuild?.TimeStamp,
                        lastFaildate = p.LastFailedBuild?.TimeStamp
                    }).ToList());

            } while (true);
        }

        private static IEnumerable<JenkinsJob> DispathMethodCall(MethodCallInfo methodInfo)
        {
            if ("get-jobs".Equals(methodInfo.MethodName))
            {
                Logger.Info($"executing {methodInfo.MethodName} method");
                return Jenkins.GetJobs(_configuration);
            }

            Logger.Info("method not found, returning empty value");
            return new List<JenkinsJob>();
        }
    }
}

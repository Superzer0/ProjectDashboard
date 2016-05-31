using System.Collections.Generic;
using System.Linq;
using Jenkins.Core;
using Jenkins.Domain;
using log4net;
using SampleDashboardBrokerConsumer.Communication;

namespace SampleDashboardBrokerConsumer
{
    public class Jenkins
    {
        private static IJenkinsRestClient _client;
        private static JenkinsServer _jenkinsServer;
        private static readonly ILog Logger = LogManager.GetLogger(typeof(Jenkins));

        public static IEnumerable<JenkinsJob> GetJobs(Configuration configuration)
        {
            InitServer(configuration);
            var jobs = _jenkinsServer.Node.Jobs.ToList();
            Logger.Info($"Jobs fetched: {jobs.Count}");
            return jobs;
        }

        private static void InitServer(Configuration configuration)
        {
            var factory = new JenkinsRestFactory();
            _client = factory.GetClient();
            _jenkinsServer = _client.GetServerAsync(configuration.JenkinsAddress, true).Result;
        }
    }
}

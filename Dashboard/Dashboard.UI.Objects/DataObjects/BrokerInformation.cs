using System;

namespace Dashboard.UI.Objects.DataObjects
{
    public class BrokerStats
    {
        public string Version { get; set; }

        public TimeSpan Uptime { get; set; }

        public int PluginsCount { get; set; }

        public string SystemInfo { get; set; }

        public string ExecutionPath { get; set; }

        public string EndpointAddress { get; set; }
    }
}

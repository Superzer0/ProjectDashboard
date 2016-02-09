using System;
using System.Collections.Generic;
using Dashboard.Broker.Objects.DataObjects;

namespace Dashboard.Broker.Objects.Providers
{
    public interface IProvidePlugins : IDisposable
    {
        BrokerPlugin AddPlugin(BrokerPlugin brokerPlugin);
        void SavePlugin(BrokerPlugin plugin);
        BrokerPlugin GetPlugin(string id, string version);
        IEnumerable<BrokerPlugin> GetPlugins();
    }
}

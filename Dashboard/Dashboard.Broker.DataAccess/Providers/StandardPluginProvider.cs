using System;
using System.Collections.Generic;
using System.Linq;
using Common.Logging;
using Dashboard.Broker.Objects.DataObjects;
using Dashboard.Broker.Objects.Providers;

namespace Dashboard.Broker.DataAccess.Providers
{
    internal class StandardPluginProvider : IProvidePlugins
    {
        private readonly ILog _logger = LogManager.GetLogger<StandardPluginProvider>();
        private readonly PluginsContext _pluginsContext;
        public StandardPluginProvider()
        {
            _pluginsContext = new PluginsContext();
            _pluginsContext.Database.Log = message => _logger.Debug(message);
        }

        public BrokerPlugin AddPlugin(BrokerPlugin brokerPlugin)
        {
            _pluginsContext.Plugins.Add(brokerPlugin);
            _pluginsContext.SaveChanges();
            return brokerPlugin;
        }

        public void SavePlugin(BrokerPlugin plugin)
        {
            if (!_pluginsContext.Set<BrokerPlugin>().Local.Contains(plugin))
            {
                _pluginsContext.Plugins.Attach(plugin);
            }
            _pluginsContext.SaveChanges();
        }

        public BrokerPlugin GetPlugin(string id, string version)
        {
            return _pluginsContext.Plugins.Find(id, version);
        }

        public IEnumerable<BrokerPlugin> GetPlugins()
        {
            return _pluginsContext.Plugins.ToList();
        }

        protected void Dispose(bool disposing)
        {
            if (!disposing) return;

            _pluginsContext?.Dispose();
            GC.SuppressFinalize(this);
        }

        public void Dispose()
        {
            Dispose(true);
        }
    }
}

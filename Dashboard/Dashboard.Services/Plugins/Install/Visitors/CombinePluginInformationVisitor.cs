using System;
using System.Collections.Generic;
using AutoMapper;
using Common.Logging;
using Dashboard.Common.PluginXml;
using Dashboard.UI.Objects.DataObjects;
using Dashboard.UI.Objects.DataObjects.Extract;
using Dashboard.UI.Objects.Services.Plugins.Extract.Visitors;

namespace Dashboard.Services.Plugins.Install.Visitors
{
    internal class CombinePluginInformationVisitor : IProcessPluginInformationVisitor
    {
        private readonly ILog _logger = LogManager.GetLogger<CombinePluginInformationVisitor>();
        private bool _resultReady = false;

        private IList<PluginMethod> _pluginMethods = new List<PluginMethod>();
        private Plugin _plugin = new Plugin();

        public IList<PluginMethod> PluginMethods
        {
            get
            {
                if (!_resultReady) throw new InvalidOperationException("To get result call Visit method before");
                return _pluginMethods;
            }
            private set { _pluginMethods = value; }
        }

        public Plugin Plugin
        {
            get
            {
                if (!_resultReady) throw new InvalidOperationException("To get result call Visit method before");
                return _plugin;
            }
            private set { _plugin = value; }
        }

        public void Visit(PluginZipBasicInformation leaf)
        {
            Mapper.Map(leaf, _plugin);
            _plugin.Added = DateTime.Now;
            _resultReady = true;
        }

        public void Visit(PluginXmlInfo leaf)
        {
            Mapper.Map(leaf.PluginXml, _plugin);

            _plugin.Xml = leaf.RawXml;

            foreach (var pluginXmlMethod in leaf.PluginXml?.XmlMethods ?? new PluginXmlMethod[] { })
            {
                var newPluginMethod = new PluginMethod { PluginId = _plugin.Id, PluginVersion = _plugin.Version };
                Mapper.Map(pluginXmlMethod, newPluginMethod);
                _pluginMethods.Add(newPluginMethod);
            }

            _resultReady = true;
        }

        public void Visit(PluginConfigurationInfo leaf)
        {
            _plugin.Configuration = leaf.ConfigurationJson;
            _resultReady = true;
        }
    }
}

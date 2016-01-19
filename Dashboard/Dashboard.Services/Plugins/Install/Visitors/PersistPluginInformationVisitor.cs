using System;
using Common.Logging;
using Dashboard.DataAccess;
using Dashboard.UI.Objects.DataObjects.Extract;
using Dashboard.UI.Objects.Services.Plugins.Extract.Visitors;

namespace Dashboard.Services.Plugins.Install.Visitors
{
    class PersistPluginInformationVisitor : IProcessPluginInformationVisitor
    {
        private readonly PluginsContext _pluginsContext;
        private readonly ILog _logger = LogManager.GetLogger<PersistPluginInformationVisitor>();

        public PersistPluginInformationVisitor(PluginsContext pluginsContext)
        {
            _pluginsContext = pluginsContext;
        }

        public void Visit(PluginZipBasicInformation leaf)
        {
            _logger.Info(m=>m("Saved info about plugin zip information"));
        }

        public void Visit(PluginXmlInfo leaf)
        {
            _logger.Info(m => m("Saved info about plugin xml file"));
        }

        public void Visit(PluginConfigurationInfo leaf)
        {
            _logger.Info(m => m("Saved info about plugin json configuration file"));
        }
    }
}

using System;
using Dashboard.UI.Objects.DataObjects.Extract;
using Dashboard.UI.Objects.Services.Plugins.Extract.Visitors;

namespace Dashboard.Services.Plugins.Install.Visitors
{
    class PersistPluginInformationVisitor : IProcessPluginInformationVisitor
    {
        public void Visit(PluginZipBasicInformation leaf)
        {
            throw new NotImplementedException();
        }

        public void Visit(PluginXmlInfo leaf)
        {
            throw new NotImplementedException();
        }

        public void Visit(PluginConfigurationInfo leaf)
        {
            throw new NotImplementedException();
        }
    }
}

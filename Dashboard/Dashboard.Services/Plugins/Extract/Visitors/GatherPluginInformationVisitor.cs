using System;
using Dashboard.UI.Objects;
using Dashboard.UI.Objects.DataObjects.Extract;
using Dashboard.UI.Objects.Services.Plugins.Extract.Visitors;
using Dashboard.UI.Objects.Services.Plugins.Install;

namespace Dashboard.Services.Plugins.Extract.Visitors
{
    internal class GatherPluginInformationVisitor : IProcessPluginInformationVisitor
    {
        private readonly PluginInformation _pluginInformation = new PluginInformation();

        public void Visit(PluginZipBasicInformation leaf)
        {
            _pluginInformation.ZipInfo = leaf;
        }

        public void Visit(PluginXmlInfo leaf)
        {
            _pluginInformation.Name = leaf.PluginXml?.Name;
            _pluginInformation.PluginId = leaf.PluginXml?.PluginId;
            _pluginInformation.Version = leaf.PluginXml?.Version;
            _pluginInformation.CommunicationType = (CommunicationType)Enum.Parse(typeof(CommunicationType), leaf.PluginXml?.CommunicationType);
            _pluginInformation.StartingProgram = leaf.PluginXml?.StartingProgram;
            _pluginInformation.MethodsCount = leaf.PluginXml?.XmlMethods?.Length ?? 0;
            _pluginInformation.RawXml = leaf.RawXml;
        }

        public void Visit(PluginConfigurationInfo leaf)
        {
            _pluginInformation.ConfigurationJson = leaf.ConfigurationJson;
        }

        public void Visit(CheckSumPluginInformation leaf)
        {
            _pluginInformation.CheckSum = leaf.CheckSum;
        }

        public PluginInformation Result => _pluginInformation;
    }
}

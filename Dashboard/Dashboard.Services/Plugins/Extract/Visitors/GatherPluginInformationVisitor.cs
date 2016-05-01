using System;
using System.Linq;
using Dashboard.Common;
using Dashboard.UI.Objects.DataObjects.Extract;
using Dashboard.UI.Objects.Providers;
using Dashboard.UI.Objects.Services.Plugins.Extract.Visitors;
using Dashboard.UI.Objects.Services.Plugins.Install;

namespace Dashboard.Services.Plugins.Extract.Visitors
{
    /// <summary>
    /// Visitor for gathering information from objects that implement BasePluginInformation
    /// </summary>
    /// <seealso cref="Dashboard.UI.Objects.Services.Plugins.Extract.Visitors.IProcessPluginInformationVisitor" />
    internal class GatherPluginInformationVisitor : IProcessPluginInformationVisitor
    {
        private readonly IProvidePlugins _providePlugins;
        private readonly PluginInformation _pluginInformation = new PluginInformation();

        public GatherPluginInformationVisitor(IProvidePlugins providePlugins)
        {
            _providePlugins = providePlugins;
        }

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

        public void CombineData()
        {
            var versions = _providePlugins.GetPluginVersions(_pluginInformation.PluginId);
            if (versions == null || !versions.Any())
            {
                _pluginInformation.IsUpdate = false;
            }
            else
            {
                _pluginInformation.IsUpdate = true;
            }
        }

        public PluginInformation Result => _pluginInformation;
    }
}

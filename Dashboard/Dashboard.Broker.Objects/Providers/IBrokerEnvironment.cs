using System;
using Dashboard.Broker.Objects.DataObjects;

namespace Dashboard.Broker.Objects.Providers
{
    public interface IBrokerEnvironment
    {
        string AppRootPath { get; }
        string MapPath(string relativePath);
        string BaseAddress { get; }
        string AppVersion { get; }
        string PluginsPath { get; }
        string PluginsZippedPath { get; }
        TimeSpan Uptime { get; }
        string SystemInfo { get; }
        string GetPluginRelativeExecPath(BrokerPlugin brokerPlugin);
        string GetPluginRelativeInstallationPath(BrokerPlugin brokerPlugin);
        string GetPluginRelativeZippedPath(BrokerPlugin brokerPlugin);
    }
}

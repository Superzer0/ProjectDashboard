using System;
using Common.Logging;
using Dashboard.UI.Objects.Providers;

namespace Dashboard.DataAccess.Providers
{
    internal class ConfigurationProvider : IConfigureDashboard
    {
        private readonly PluginsContext _pluginsContext;
        private readonly ILog _logger = LogManager.GetLogger<ConfigurationProvider>();

        public ConfigurationProvider(PluginsContext pluginsContext)
        {
            _pluginsContext = pluginsContext;
        }

        public bool GetAdminPartyState()
        {
            var adminPartyState = true;

            try
            {
                var entity = _pluginsContext.InstanceSettings.Find(SettingsKeys.AdminPartySetting);
                adminPartyState = bool.Parse(entity.Value);
            }
            catch (Exception e)
            {
                _logger.Error(e);
            }

            return adminPartyState;
        }

        public void SetAdminPartyState(bool changeStatus)
        {
            var entity = _pluginsContext.InstanceSettings.Find(SettingsKeys.AdminPartySetting);
            entity.Value = changeStatus.ToString();
            _pluginsContext.SaveChanges();
        }

        private static class SettingsKeys
        {
            public const string AdminPartySetting = "admin_party_state";
        }
    }
}

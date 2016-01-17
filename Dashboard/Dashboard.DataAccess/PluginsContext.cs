using System.Data.Entity;
using Dashboard.UI.Objects.DataObjects;

namespace Dashboard.DataAccess
{
    public class PluginsContext : DbContext
    {
        public PluginsContext() : base("name=EmbeddedDbContext")
        {
            Database.SetInitializer<PluginsContext>(null);
        }

        public DbSet<Plugin> Plugins { get; set; }
        public DbSet<PluginUiConfiguration> PluginUiConfigurations { get; set; }
        public DbSet<InstanceSetting> InstanceSettings { get; set; }
    }
}

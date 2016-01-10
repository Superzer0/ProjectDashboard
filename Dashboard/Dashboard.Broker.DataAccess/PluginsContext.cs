using System.Data.Entity;
using Dashboard.Broker.Objects.DataObjects;

namespace Dashboard.Broker.DataAccess
{
    public class PluginsContext : DbContext
    {
        public PluginsContext() : base("name=EmbeddedDbContext")
        {

            Database.SetInitializer<PluginsContext>(null);
        }

        public DbSet<Plugin> Plugins { get; set; }
        public DbSet<PluginInternalConfiguration> PluginInternalConfigurations { get; set; }
    }
}

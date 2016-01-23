using System.Data.Entity;
using Dashboard.UI.Objects.DataObjects;

namespace Dashboard.DataAccess
{
    public class PluginsContext : DbContext
    {
        public PluginsContext() : base("name=EmbeddedDbContext")
        {
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Plugin>()
                .HasMany(p => p.PluginMethods)
                .WithRequired(p => p.Plugin)
                .HasForeignKey(p => new { p.PluginId, p.PluginVersion })
                .WillCascadeOnDelete(true);
        }

        public DbSet<Plugin> Plugins { get; set; }
        public DbSet<PluginMethod> PluginMethods { get; set; }
        public DbSet<PluginUiConfiguration> PluginUiConfigurations { get; set; }
        public DbSet<InstanceSetting> InstanceSettings { get; set; }
    }
}

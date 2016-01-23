using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dashboard.UI.Objects.DataObjects
{
    public class PluginMethod
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Column("Plugin_Id", Order = 0)]
        public Guid PluginId { get; set; }

        [Column("Plugin_Version", Order = 1)]
        public string PluginVersion { get; set; }

        [MaxLength(100)]
        public string Name { get; set; }

        public InputType InputType { get; set; }
        public InputType OutputType { get; set; }

        public virtual Plugin Plugin { get; set; }

        public override string ToString()
        {
            return
                $"Id: {Id}, PluginId: {PluginId}, PluginVersion: {PluginVersion}, Name: {Name}, InputType: {InputType}, OutputType: {OutputType}";
        }
    }
}

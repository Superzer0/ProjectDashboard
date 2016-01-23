using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dashboard.UI.Objects.DataObjects
{
    [Table("Plugins")]
    public class Plugin
    {
        public Plugin()
        {
            PluginMethods = new HashSet<PluginMethod>();
        }

        [Key, Column(Order = 0)]
        [Required]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Key, Column(Order = 1)]
        [Required]
        [MaxLength(19)]
        public string Version { get; set; }

        [MaxLength(100)]
        public string Name { get; set; }

        public CommunicationType CommunicationType { get; set; }

        [MaxLength(50)]
        public string StartingProgram { get; set; }

        [MaxLength(4000)]
        public string Xml { get; set; }

        [MaxLength(4000)]
        public string Configuration { get; set; }

        public DateTime Added { get; set; }

        [MaxLength(100)]
        public string AddedBy { get; set; }

        public virtual ICollection<PluginMethod> PluginMethods { get; set; }

        public override string ToString()
        {
            return
                $"Id: {Id}, Version: {Version}, Name: {Name}, CommunicationType: {CommunicationType}, StartingProgram: {StartingProgram}, Added: {Added}, AddedBy: {AddedBy}, PluginMethods: {PluginMethods}";
        }
    }
}

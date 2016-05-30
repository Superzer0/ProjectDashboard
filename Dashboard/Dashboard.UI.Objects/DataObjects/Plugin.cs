using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Dashboard.Common;

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
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Required]
        public string Id { get; set; }

        [Key, Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
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

        public bool Disabled { get; set; }

        public string Icon { get; set; }

        public long ArchiveSize { get; set; }

        public int FilesCount { get; set; }

        public long UncompressedSize { get; set; }

        public virtual ICollection<PluginMethod> PluginMethods { get; set; }

        public string UrlName => GetUrlName(Name, Id, Version);

        public string IconUrl(string pluginPath)
        {
            return $"{pluginPath}/{UrlName}/{Icon}".TrimStart('~');
        }

        public static string GetUrlName(string name, string id, string version) => $"{name.Replace(" ", "-")}_{id}_{version.Replace(".", "-")}";

        public static string GetUniqueName(string id, string version) => $"{id}-{version}";

        public override string ToString()
        {
            return
                $"Id: {Id}, Version: {Version}, Name: {Name}, CommunicationType: {CommunicationType}, StartingProgram: {StartingProgram}, Added: {Added}, AddedBy: {AddedBy}, PluginMethods: {PluginMethods}";
        }
    }
}

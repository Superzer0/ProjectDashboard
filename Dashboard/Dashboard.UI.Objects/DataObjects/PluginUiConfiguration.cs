using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dashboard.UI.Objects.DataObjects
{
    [Table("PluginsUiConfiguration")]
    public class PluginUiConfiguration
    {
        [Key, Column(Order = 0)]
        public Guid Id { get; set; }

        [Key, Column(Order = 1)]
        public string Version { get; set; }

        [Key, Column(Order = 2)]
        public Guid UserId { get; set; }

        [MaxLength(4000)]
        public string JsonConfiguration { get; set; }
    }
}

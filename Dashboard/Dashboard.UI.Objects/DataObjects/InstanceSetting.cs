using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dashboard.UI.Objects.DataObjects
{
    [Table("InstanceSettings")]
    public class InstanceSetting
    {
        [Key]
        [Required]
        public string Id { get; set; }

        [Required]
        public string Value { get; set; }
    }
}

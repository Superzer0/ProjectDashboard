using System.ComponentModel.DataAnnotations;

namespace Dashboard.UI.Objects.DataObjects
{
    public class InstanceSetting
    {
        [Required]
        public string Id { get; set; }

        [Required]
        public string Value { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;
using Dashboard.UI.Objects.Auth;

namespace Dashboard.Models.Account
{
    public class CreateAppViewModel
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        public AuthApplicationType ApplicationType { get; set; }

        [MaxLength(100)]
        public string AllowedOrigin { get; set; }
    }
}

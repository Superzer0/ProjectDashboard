using System.ComponentModel.DataAnnotations;

namespace Dashboard.Models.Account
{
    public class ChangeRolesViewModel
    {
        [Required]
        public string[] RolesToRemove { get; set; }

        [Required]
        public string[] RolesToAdd { get; set; }
    }
}

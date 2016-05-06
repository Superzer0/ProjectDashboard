using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dashboard.UI.Objects.Auth
{
    [Table("AuthClients")]
    public class AuthClient
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string Id { get; set; }

        [Required]
        public string Secret { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        public AuthApplicationType ApplicationType { get; set; }

        public bool Active { get; set; }

        public int RefreshTokenLifeTime { get; set; }
        public DateTime CreatedAt { get; set; }
        [Required]
        [MaxLength(100)]
        public string AllowedOrigin { get; set; }
    }
}

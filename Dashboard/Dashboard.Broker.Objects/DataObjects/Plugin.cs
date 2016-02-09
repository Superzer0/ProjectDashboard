using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Dashboard.Common;

namespace Dashboard.Broker.Objects.DataObjects
{
    [Table("Plugins")]
    public class BrokerPlugin
    {
        [Required]
        [Key, Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string Id { get; set; }

        [Required]
        [MaxLength(19)]
        [Key, Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string Version { get; set; }

        [MaxLength(100)]
        public string Name { get; set; }

        public CommunicationType CommunicationType { get; set; }

        [MaxLength(50)]
        public string StartingProgram { get; set; }

        public string CheckSum { get; set; }

        public string ExecutablePath { get; set; }

        public string UrlName => GetUrlName(Name, Id, Version);

        public static string GetUrlName(string name, string id, string version) => $"{name.Replace(" ", "-")}_{id}_{version.Replace(".", "-")}";

        public override string ToString()
        {
            return
                $"Id: {Id}, Version: {Version}, Name: {Name}, CommunicationType: {CommunicationType}, StartingProgram: {StartingProgram}";
        }

    }
}

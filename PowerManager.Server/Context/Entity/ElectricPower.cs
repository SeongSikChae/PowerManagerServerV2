using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PowerManager.Server.Context.Entity
{
    [Table("ElectricPower")]
    public class ElectricPower
    {
        [Key]
        public ElectricPowerType PowerType { get; set; }

        [Required, StringLength(20)]
        public string PowerTypeName { get; set; } = null!;
    }
}

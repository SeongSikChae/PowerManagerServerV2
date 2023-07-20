using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace PowerManagerServer.Context
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum ElectricPowerType
    {
        HouseLow,
        HouseHigh
    }

    [Table("ElectricPower")]
    public sealed class ElectricPower
    {
        [Key]
        public ElectricPowerType PowerType { get; set; }

        [Required, StringLength(20)]
        public string PowerTypeName { get; set; } = string.Empty;
    }

    [Table("PowerTypePrice")]
    public sealed class PowerTypePrice
    {
        [Key, Required]
        public ElectricPowerType PowerType { get; set; }

        [Key, Required]
        public int Level { get; set; }

        [Required]
        public double Price { get; set; }
    }
}

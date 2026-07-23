using System.Text.Json.Serialization;

namespace PowerManager.Server.Context.Entity
{
    [JsonConverter(typeof(JsonStringEnumConverter<ElectricPowerType>))]
    public enum ElectricPowerType
    {
        HouseLow,
        HouseHigh
    }
}

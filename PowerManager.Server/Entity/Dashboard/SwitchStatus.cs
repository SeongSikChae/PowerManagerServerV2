using System.Text.Json.Serialization;

namespace PowerManager.Server.Entity.Dashboard
{
    [JsonConverter(typeof(JsonStringEnumConverter<SwitchStatus>))]
    public enum SwitchStatus
    {
        Unknown, On, Off
    }
}

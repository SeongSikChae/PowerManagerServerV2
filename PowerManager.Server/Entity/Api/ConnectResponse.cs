using System.Text.Json.Serialization;

namespace PowerManager.Server.Entity.Api
{
    public sealed class ConnectResponse
    {
        [JsonPropertyName("delay")]
        public string Delay { get; set; } = "0";
    }
}

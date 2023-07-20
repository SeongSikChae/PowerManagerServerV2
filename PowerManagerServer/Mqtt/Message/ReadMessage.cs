using System.Text.Json.Serialization;

namespace PowerManagerServer.Mqtt.Message
{
    public sealed class ReadMessage
    {
        [JsonPropertyName("sid")]
        public string Sid { get; set; } = "2";

        [JsonPropertyName("device_id")]
        public string DeviceId { get; set; } = string.Empty;

        [JsonPropertyName("msg")]
        public MsgPayload? Msg { get; set; }

        public sealed class MsgPayload
        {
            [JsonPropertyName("o")]
            public string O { get; set; } = "r";

            [JsonPropertyName("e")]
            public List<Body> E { get; set; } = new List<Body>();

            public sealed class Body
            {
                [JsonPropertyName("n")]
                public string N { get; set; } = string.Empty;

                [JsonPropertyName("ti")]
                public string Time { get; set; } = string.Empty; 
            }
        }
    }
}

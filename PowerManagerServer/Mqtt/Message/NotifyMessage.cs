using System.Text.Json.Serialization;

namespace PowerManagerServer.Mqtt.Message
{
    public sealed class NotifyMessage
    {
        [JsonPropertyName("sid")]
        public string Sid { get; set; } = "2";

        [JsonPropertyName("msg")]
        public MsgPayload? Msg { get; set; }

        public sealed class MsgPayload
        {
            [JsonPropertyName("o")]
            public string O { get; set; } = "n";

            [JsonPropertyName("e")]
            public List<Body> E { get; set; } = new List<Body>();

            public sealed class Body
            {
                [JsonPropertyName("n")]
                public string? N { get; set; }

                [JsonPropertyName("sv")]
                public string? Sv { get; set; }

                [JsonPropertyName("ti")]
                public string? Time { get; set; }
            }
        }
    }
}

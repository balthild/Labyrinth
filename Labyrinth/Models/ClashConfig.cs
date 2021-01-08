using System.Text.Json.Serialization;

namespace Labyrinth.Models {
    public class ClashConfig {
        [JsonPropertyName("port")]
        public ushort Port { get; set; } = 0;

        [JsonPropertyName("socks-port")]
        public ushort SocksPort { get; set; } = 0;

        [JsonPropertyName("redir-port")]
        public ushort RedirPort { get; set; } = 0;

        [JsonPropertyName("mixed-port")]
        public ushort MixedPort { get; set; } = 0;

        [JsonPropertyName("allow-lan")]
        public bool AllowLan { get; set; } = false;

        [JsonPropertyName("mode")]
        public string Mode { get; set; } = "";

        [JsonPropertyName("log-level")]
        public string LogLevel { get; set; } = "";
    }
}

using System.Text.Json.Serialization;

namespace Labyrinth.Models {
    public class Adapter {
        public static readonly string[] GroupTypes = {
            "URLTest", "Fallback", "LoadBalance", "Selector"
        };

        public static readonly string[] ProxyTypes = {
            "Direct", "Reject", "Shadowsocks", "Snell", "Vmess", "Socks5", "Http"
        };

        public class HistoryEntry {
            [JsonPropertyName("time")]
            public string Time { get; set; } = "";

            [JsonPropertyName("delay")]
            public int Delay { get; set; }
        }

        [JsonIgnore]
        public string Name { get; set; } = "";

        [JsonPropertyName("type")]
        public string Type { get; set; } = "";

        [JsonPropertyName("now")]
        public string Now { get; set; } = "";

        [JsonPropertyName("all")]
        public string[] All { get; set; } = { };

        [JsonPropertyName("history")]
        public HistoryEntry[] History { get; set; } = { };
    }
}

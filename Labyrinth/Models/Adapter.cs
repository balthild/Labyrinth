using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace Labyrinth.Models {
    public class Adapter {
        // See clash/constant/adapters.go
        private static readonly IEnumerable<string> GroupTypes = new [] {
            "Relay",
            "Selector",
            "Fallback",
            "URLTest",
            "LoadBalance",
        };

        private static readonly IEnumerable<string> ProxyTypes = new [] {
            "Direct",
            "Reject",
            "Shadowsocks",
            "ShadowsocksR",
            "Snell",
            "Trojan",
            "Vmess",
            "Socks5",
            "Http",
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

        public bool IsGroup() => GroupTypes.Contains(Type);

        public bool IsProxy() => ProxyTypes.Contains(Type);
    }
}

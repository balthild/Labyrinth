using System.Text.Json.Serialization;

namespace Labyrinth.Models {
    public class ProxyGroup {
        public static readonly string[] ValidTypes = { "URLTest", "Fallback", "LoadBalance", "Selector" };

        [JsonIgnore]
        public string Name { get; set; } = "";

        [JsonPropertyName("type")]
        public string Type { get; set; } = "";

        [JsonPropertyName("now")]
        public string Now { get; set; } = "";

        [JsonPropertyName("all")]
        public string[] All { get; set; } = { };
    }
}

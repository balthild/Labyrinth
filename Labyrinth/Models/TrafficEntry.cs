using System.Text.Json.Serialization;

namespace Labyrinth.Models {
    public class TrafficEntry {
        [JsonPropertyName("up")]
        public long Up { get; set; }

        [JsonPropertyName("down")]
        public long Down { get; set; }
    }
}

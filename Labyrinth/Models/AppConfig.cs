using System.Collections.Generic;

namespace Labyrinth.Models {
    public class AppConfig {
        public string ConfigFile { get; set; } = "config.yaml";

        public string Mode { get; set; } = "rule";

        public Dictionary<string, Subscription> Subscriptions { get; set; } = new();

        // [config -> [group -> adapter]]
        public Dictionary<string, Dictionary<string, string>> SelectedAdapters { get; set; } = new();
    }
}

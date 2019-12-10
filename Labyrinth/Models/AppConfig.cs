using System.Collections.Generic;

namespace Labyrinth.Models {
    public class AppConfig {
        public string ConfigFile { get; set; } = "config.yaml";

        public Dictionary<string, Subscription> Subscriptions { get; set; } = new Dictionary<string, Subscription>();
    }
}

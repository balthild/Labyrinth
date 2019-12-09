using System.Collections.Generic;

namespace Labyrinth.Models {
    public class AppConfig {
        public string ConfigFile { get; set; } = "config.yaml";

        public Dictionary<string, Subscription> Subscriptions = new Dictionary<string, Subscription>();
    }
}

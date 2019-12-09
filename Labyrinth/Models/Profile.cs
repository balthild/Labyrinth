using System;
using Labyrinth.Support;

namespace Labyrinth.Models {
    public class Profile {
        public string Name { get; set; } = "";

        public Subscription? Subscription { get; set; }

        public string Description => Name switch {
            "config.yaml" => "Main configuration",
            "config.yml" => "Main configuration",
            _ => Subscription switch {
                null => "Custom profile",
                _ => $"Subscription - Last updated at {Utils.FormatTime(Subscription.UpdatedAt)}  {Subscription.Url}"
            }
        };
    }
}

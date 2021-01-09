using System;
using Labyrinth.Support;
using Labyrinth.Support.Interop;

namespace Labyrinth.Models {
    public class Profile {
        public string Name { get; set; } = "";

        public Subscription? Subscription { get; set; }

        public Clash.ConfigStats? Stats { get; set; }

        public string Description => Name switch {
            "config.yaml" => "Main configuration",
            "config.yml" => "Main configuration",
            _ => Subscription switch {
                null => "Custom profile",
                _ => $"Subscription - Updated {Utils.FormatRelativeTimeAgo(Subscription.UpdatedAt)}",
            },
        };

        public string StatsDescription => Stats switch {
            null => "(Not a valid clash config)",
            // ReSharper disable once UseStringInterpolation
            { ProxyCount: var p, ProxyGroupCount: var g, RuleCount: var r } => string.Format(
                "{0} {1}, {2} {3}, {4} {5}",
                p, p > 1 ? "proxies" : "proxy",
                g, g > 1 ? "groups" : "group",
                r, r > 1 ? "rules" : "rule"
            ),
        };
    }
}

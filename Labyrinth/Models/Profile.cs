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
            { ProxyCount: var p, ProxyGroupCount: var g, RuleCount: var r } =>
                $"{p} proxies, {g} groups, {r} rules",
        };
    }
}

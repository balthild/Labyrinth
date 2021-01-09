using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Labyrinth.Models;
using Labyrinth.Support.Interop;

namespace Labyrinth.Support {
    public static class ConfigFile {
        public static string GetPath(string name) => Path.Combine(Clash.ConfigDir, name);

        public static Task ExtractDefaultClashConfig(string path) =>
            Utils.ExtractResource("Labyrinth.Resources.config.yaml", path);

        public static IEnumerable<string> GetClashConfigs() =>
            Directory.EnumerateFiles(Clash.ConfigDir)
                .Select(x => Path.GetFileName(x)!)
                .Where(x => x.EndsWith(".yml") || x.EndsWith(".yaml"))
                .OrderBy(x => x switch {
                    "config.yaml" => 1,
                    _ => 99,
                });

        public static async Task SaveAppConfig(AppConfig config) {
            var options = new JsonSerializerOptions { WriteIndented = true };
            string json = JsonSerializer.Serialize(config, options);
            await File.WriteAllTextAsync(GetPath("Labyrinth.json"), json);
        }
    }
}

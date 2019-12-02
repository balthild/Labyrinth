using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Labyrinth.Support.Interop;
using Labyrinth.ViewModels;

namespace Labyrinth.Support {
    public static class ConfigFile {
        public static string GetPath(string name) => Path.Combine(Clash.ConfigDir, name);

        public static Task ExtractDefaultClashConfig(string path) =>
            Utils.ExtractResource("Labyrinth.Resources.config.yaml", path);

        public static IEnumerable<string> GetClashConfigs() =>
            Directory.EnumerateFiles(Clash.ConfigDir)
                .Select(x => Path.GetFileName(x)!)
                .Where(x => x.EndsWith(".yml") || x.EndsWith(".yaml"));

        public static Task SaveCurrentAppConfig() =>
            File.WriteAllTextAsync(GetPath("labyrinth.json"), JsonSerializer.Serialize(ViewModelBase.State.AppConfig));
    }
}

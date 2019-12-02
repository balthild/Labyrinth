using System.IO;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;

namespace Labyrinth.Support {
    public static class ConfigFile {
        public static Task ExtractDefaultClashConfig(string path) {
            const string res = "Labyrinth.Resources.config.yaml";

            Assembly assembly = Assembly.GetEntryAssembly()!;
            using Stream resStream = assembly.GetManifestResourceStream(res)!;
            using FileStream fileStream = File.OpenWrite(path);
            return resStream.CopyToAsync(fileStream);
        }

        public static Task ExtractDefaultAppConfig(string path) {
            const string res = "Labyrinth.Resources.labyrinth.json";

            Assembly assembly = Assembly.GetEntryAssembly()!;
            using Stream resStream = assembly.GetManifestResourceStream(res)!;
            using FileStream fileStream = File.OpenWrite(path);
            return resStream.CopyToAsync(fileStream);
        }
    }
}

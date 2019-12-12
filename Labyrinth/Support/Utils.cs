using System;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;

namespace Labyrinth.Support {
    public static class Utils {
        public static readonly HttpClient HttpClient = new HttpClient();

        static Utils() {
            HttpClient.DefaultRequestHeaders.ConnectionClose = true;
        }

        private static readonly string[] SizeSuffixes = {
            // long.MaxValue B = 8 EiB
            "B", "KiB", "MiB", "GiB", "TiB", "PiB", "EiB"
        };

        public static string BinarySize(long bytes) {
            if (bytes < 0) {
                return "Invalid";
            }

            double num = bytes;

            short i = 0;
            long breakpoint = 1 << 10;
            while (i < 6 && breakpoint <= bytes) {
                i++;
                breakpoint <<= 10;

                num /= 1024;
            }

            return $"{num:0.#} {SizeSuffixes[i]}";
        }

        public static string FormatTime(long ts) =>
            DateTimeOffset.FromUnixTimeSeconds(ts).LocalDateTime.ToString("yyyy'-'MM'-'dd HH':'mm");

        public static async Task ExtractResource(string resource, string path) {
            var assembly = Assembly.GetEntryAssembly()!;
            await using Stream resStream = assembly.GetManifestResourceStream(resource)!;
            await using FileStream fileStream = File.OpenWrite(path);
            await resStream.CopyToAsync(fileStream).ConfigureAwait(false);
            // We have to await the Task, otherwise the streams will be disposed after return
        }

        public static string RemoveYamlExt(string filename) {
            if (filename.EndsWith(".yaml"))
                return filename.Substring(0, filename.Length - 5);
            else if (filename.EndsWith("yml"))
                return filename.Substring(0, filename.Length - 4);
            else
                return filename;
        }
    }
}

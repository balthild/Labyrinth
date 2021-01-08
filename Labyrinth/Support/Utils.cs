using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;

namespace Labyrinth.Support {
    public static class Utils {
        private static readonly object ClientLock = new();

        private static HttpClient httpClient = new() {
            DefaultRequestHeaders = {
                ConnectionClose = true,
            },
        };

        public static HttpClient GetProxiedHttpClient() {
            HttpClient client;
            lock (ClientLock) {
                client = httpClient;
            }

            return client;
        }

        public static void UpdateProxiedHttpClient(int httpProxyPort) {
            var handler = new HttpClientHandler {
                UseProxy = true,
                Proxy = new WebProxy("127.0.0.1", httpProxyPort),
            };

            var client = new HttpClient(handler) {
                DefaultRequestHeaders = {
                    ConnectionClose = true,
                },
            };

            lock (ClientLock) {
                httpClient = client;
            }
        }

        private static readonly string[] SizeSuffixes = {
            // long.MaxValue B = 8 EiB
            "B", "KiB", "MiB", "GiB", "TiB", "PiB", "EiB",
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

        private const int MINUTE = 60;
        private const int HOUR = 60 * MINUTE;
        private const int DAY = 24 * HOUR;
        private const int MONTH = 30 * DAY;

        public static string FormatRelativeTimeAgo(long ts) {
            long deltaSeconds = DateTimeOffset.Now.ToUnixTimeSeconds() - ts;
            TimeSpan delta = TimeSpan.FromSeconds(deltaSeconds);

            return deltaSeconds switch {
                var ds when ds < 0 => $"at {FormatTime(ts)}",
                var ds when ds < 1 * MINUTE => "just now",
                var ds when ds < 1.5 * MINUTE => "a minute ago",
                var ds when ds < 1 * HOUR => $"{delta.Minutes} minutes ago",
                var ds when ds < 1.5 * HOUR => "an hour ago",
                var ds when ds < 1 * DAY => $"{delta.Hours} hour ago",
                var ds when ds < 1.5 * DAY => "a day ago",
                var ds when ds < 1 * MONTH => $"{delta.Days} days ago",
                _ => DateTimeOffset.FromUnixTimeSeconds(ts).LocalDateTime.ToString("'at 'yyyy'-'MM'-'dd"),
            };
        }

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
            else if (filename.EndsWith(".yml"))
                return filename.Substring(0, filename.Length - 4);
            else
                return filename;
        }
    }
}

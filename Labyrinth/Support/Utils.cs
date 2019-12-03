using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Threading.Tasks;
using Labyrinth.Models;

namespace Labyrinth.Support {
    public static class Utils {
        private static readonly HttpClient ControllerClient = new HttpClient();

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

        public static async Task ExtractResource(string resource, string path) {
            Assembly assembly = Assembly.GetEntryAssembly()!;
            await using Stream resStream = assembly.GetManifestResourceStream(resource)!;
            await using FileStream fileStream = File.OpenWrite(path);
            await resStream.CopyToAsync(fileStream).ConfigureAwait(false);
            // We have to await the Task, otherwise the streams will be disposed after return
        }

        public static void UpdateControllerClient(ClashController controller) {
            ControllerClient.BaseAddress = new Uri("http://" + controller.Address);
            ControllerClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", controller.Secret);
        }

        public static Task<HttpResponseMessage> RequestController(HttpMethod method, string path, string body = "") {
            var uri = new Uri(path, UriKind.Relative);
            var message = new HttpRequestMessage(method, uri);

            if (method != HttpMethod.Get && method != HttpMethod.Head) {
                message.Content = new StringContent(body);
            }

            return ControllerClient.SendAsync(message);
        }

        public static Task<Stream> RequestStreamController(string path) {
            var uri = new Uri(path, UriKind.Relative);
            return ControllerClient.GetStreamAsync(uri);
        }
    }
}

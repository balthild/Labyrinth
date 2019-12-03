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
    }
}

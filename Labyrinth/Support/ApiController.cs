using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Labyrinth.Models;

namespace Labyrinth.Support {
    public static class ApiController {
        private static readonly object ClientLock = new();

        private static HttpClient controllerClient = new();

        private static HttpClient GetClient() {
            HttpClient client;
            lock (ClientLock) {
                client = controllerClient;
            }

            return client;
        }

        public static void UpdateClient(ClashController controller) {
            // The first request may become extremely slow if localhost is resolved to IPv6
            string address = controller.Address.Replace("localhost:", "127.0.0.1:");

            // Modifying the properties on the existing client is not thread-safe, so we need to create new ones
            var handler = new HttpClientHandler { UseProxy = false };
            var client = new HttpClient(handler) {
                BaseAddress = new Uri($"http://{address}"),
                DefaultRequestHeaders = {
                    Authorization = new AuthenticationHeaderValue("Bearer", controller.Secret),
                },
            };

            lock (ClientLock) {
                controllerClient = client;
            }
        }

        public static Task<HttpResponseMessage> Request(HttpMethod method, string path, string body = "") {
            var uri = new Uri(path, UriKind.Relative);
            var message = new HttpRequestMessage(method, uri);

            if (method != HttpMethod.Get && method != HttpMethod.Head) {
                message.Content = new StringContent(body);
            }

            return GetClient().SendAsync(message);
        }

        public static Task<Stream> PollStream(string path) {
            var uri = new Uri(path, UriKind.Relative);
            return GetClient().GetStreamAsync(uri);
        }
    }
}

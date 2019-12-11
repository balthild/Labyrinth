using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Labyrinth.Models;

namespace Labyrinth.Support {
    public static class ApiController {
        private static readonly HttpClient ControllerClient = new HttpClient();

        public static void UpdateClient(ClashController controller) {
            ControllerClient.BaseAddress = new Uri("http://" + controller.Address);
            ControllerClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", controller.Secret);
        }

        public static Task<HttpResponseMessage> Request(HttpMethod method, string path, string body = "") {
            var uri = new Uri(path, UriKind.Relative);
            var message = new HttpRequestMessage(method, uri);

            if (method != HttpMethod.Get && method != HttpMethod.Head) {
                message.Content = new StringContent(body);
            }

            return ControllerClient.SendAsync(message);
        }

        public static Task<Stream> PollStream(string path) {
            var uri = new Uri(path, UriKind.Relative);
            return ControllerClient.GetStreamAsync(uri);
        }
    }
}

using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Labyrinth.Models;
using Labyrinth.Support;
using ReactiveUI;

namespace Labyrinth.ViewModels {
    public class ProxyViewModel : ViewModelBase, ITabContentViewModel {
        private ProxyGroup[] proxyGroups = new ProxyGroup[0];

        public ProxyGroup[] ProxyGroups {
            get => proxyGroups;
            set => this.RaiseAndSetIfChanged(ref proxyGroups, value);
        }

        private async Task GetProxies() {
            using HttpResponseMessage message = await Utils.RequestController(HttpMethod.Get, "/proxies");
            string json = await message.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, ProxyGroup>>>(json);

            ProxyGroups = result["proxies"]
                .Where(item => ProxyGroup.ValidTypes.Contains(item.Value.Type))
                .Select(item => {
                    // Mutating in LINQ is an anti-pattern, but I can't find a more concise and elegant way
                    item.Value.Name = item.Key;
                    return item.Value;
                })
                .ToArray();
        }

        public void OnActivate() {
            Task.Run(GetProxies);
        }
    }
}

using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reactive.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Avalonia.Controls;
using Labyrinth.Models;
using Labyrinth.Support;
using ReactiveUI;

namespace Labyrinth.ViewModels {
    public class ProxyViewModel : ViewModelBase {
        private Dictionary<string, Adapter> allAdapters = new Dictionary<string, Adapter>();

        public Dictionary<string, Adapter> AllAdapters {
            get => allAdapters;
            set => this.RaiseAndSetIfChanged(ref allAdapters, value);
        }

        private ObservableAsPropertyHelper<IEnumerable<Adapter>> proxyGroups;

        public IEnumerable<Adapter> ProxyGroups => proxyGroups.Value;

        private Adapter selectedGroup = new Adapter();

        public Adapter SelectedGroup {
            get => selectedGroup;
            set => this.RaiseAndSetIfChanged(ref selectedGroup, value);
        }

        private ObservableAsPropertyHelper<IEnumerable<Adapter>> adaptersInSelectedGroup;

        public IEnumerable<Adapter> AdaptersInSelectedGroup => adaptersInSelectedGroup.Value;

        private ObservableAsPropertyHelper<Adapter> activeAdapterInSelectedGroup;

        public Adapter ActiveAdapterInSelectedGroup => activeAdapterInSelectedGroup.Value;

        public ProxyViewModel() {
            this.WhenAnyValue(x => x.AllAdapters, x => x.GlobalState.ClashConfig.Mode)
                .Select(tuple => {
                    return tuple.Item1
                        .Select(x => x.Value)
                        .Where(x => x.Name == "GLOBAL" ? tuple.Item2 == "Global" : Adapter.GroupTypes.Contains(x.Type))
                        .OrderBy(x => x.Name switch {
                            "GLOBAL" => 1,
                            "Proxy" => 2,
                            _ => 99
                        });
                })
                .ToProperty(this, nameof(ProxyGroups), out proxyGroups);

            this.WhenAnyValue(x => x.SelectedGroup, x => x.AllAdapters)
                .Select(tuple => tuple.Item1.Name switch {
                    "GLOBAL" => tuple.Item1.All
                        .Select(name => tuple.Item2.GetValueOrDefault(name)!)
                        .OrderBy(adapter =>  adapter.Type switch {
                            "Global" => 1,
                            "Direct" => 2,
                            "Reject" => 3,
                            "URLTest" => 10,
                            "Fallback" => 10,
                            "LoadBalance" => 10,
                            "Selector" => 10,
                            _ => 99
                        }),
                    _ => tuple.Item1.All.Select(name => tuple.Item2.GetValueOrDefault(name)!)
                })
                .ToProperty(this, nameof(AdaptersInSelectedGroup), out adaptersInSelectedGroup);

            this.WhenAnyValue(x => x.SelectedGroup.Now, x => x.AllAdapters)
                .Select(tuple => tuple.Item2.GetValueOrDefault(tuple.Item1)!)
                .ToProperty(this, nameof(ActiveAdapterInSelectedGroup), out activeAdapterInSelectedGroup);

            Task.Run(GetAdapters);
        }

        private async Task GetAdapters() {
            using HttpResponseMessage message = await ApiController.Request(HttpMethod.Get, "/proxies");
            string json = await message.Content.ReadAsStringAsync();

            var result = JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, Adapter>>>(json);
            var adapters = result["proxies"];

            foreach ((string key, Adapter value) in adapters) {
                value.Name = key;
            }

            AllAdapters = adapters;
        }

        public void ShowProxyGroupDetail(SelectionChangedEventArgs args) {
            SelectedGroup = args.AddedItems.Cast<Adapter>().FirstOrDefault();
        }

        public void SwitchSelectorAdapter(string newAdapterName) {
            Adapter group = SelectedGroup;
            if (group.Type == "Selector" && group.Now != newAdapterName) {
                Task.Run(async delegate {
                    string json = JsonSerializer.Serialize(new { name = newAdapterName });
                    await ApiController.Request(HttpMethod.Put, $"/proxies/{group.Name}", json);

                    this.RaisePropertyChanging(nameof(SelectedGroup));
                    group.Now = newAdapterName;
                    this.RaisePropertyChanged(nameof(SelectedGroup));
                });
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reactive;
using System.Reactive.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Avalonia.Controls;
using Labyrinth.Models;
using Labyrinth.Support;
using ReactiveUI;

namespace Labyrinth.ViewModels {
    public class ProxyViewModel : ViewModelBase, ITabContentViewModel {
        private Dictionary<string, Adapter> allAdapters = new Dictionary<string, Adapter>();

        public Dictionary<string, Adapter> AllAdapters {
            get => allAdapters;
            set => this.RaiseAndSetIfChanged(ref allAdapters, value);
        }

        private ObservableAsPropertyHelper<IEnumerable<Adapter>> proxyGroups;

        public IEnumerable<Adapter> ProxyGroups => proxyGroups.Value;

        private Adapter? selectedGroup;

        public Adapter? SelectedGroup {
            get => selectedGroup;
            set => this.RaiseAndSetIfChanged(ref selectedGroup, value);
        }

        private ObservableAsPropertyHelper<IEnumerable<Adapter>> adaptersInSelectedGroup;

        public IEnumerable<Adapter> AdaptersInSelectedGroup => adaptersInSelectedGroup.Value;

        private ObservableAsPropertyHelper<Adapter> activeAdapterInSelectedGroup;

        public Adapter ActiveAdapterInSelectedGroup => activeAdapterInSelectedGroup.Value;

        public ProxyViewModel() {
            this.WhenAnyValue(x => x.AllAdapters)
                .Select(adapters => {
                    return adapters
                        .Where(x => Adapter.GroupTypes.Contains(x.Value.Type))
                        .Select(x => x.Value);
                })
                .ToProperty(this, nameof(ProxyGroups), out proxyGroups);

            this.WhenAnyValue(x => x.SelectedGroup!.All, x => x.AllAdapters)
                .Select(tuple => tuple.Item1?.Select(name => tuple.Item2[name]) ?? Enumerable.Empty<Adapter>())
                .ToProperty(this, nameof(AdaptersInSelectedGroup), out adaptersInSelectedGroup);

            this.WhenAnyValue(x => x.SelectedGroup!.Now, x => x.AllAdapters)
                .Select(tuple => tuple.Item1 == null ? new Adapter() : tuple.Item2[tuple.Item1])
                .ToProperty(this, nameof(ActiveAdapterInSelectedGroup), out activeAdapterInSelectedGroup);
        }

        public void ShowProxyGroupDetail(SelectionChangedEventArgs args) {
            SelectedGroup = args.AddedItems.Count switch {
                0 => null,
                _ => args.AddedItems.Cast<Adapter>().First()
            };
        }

        public void SwitchSelectorAdapter(string newAdapterName) {
            Adapter? group = SelectedGroup;
            if (group != null && group.Type == "Selector") {
                Task.Run(async delegate {
                    string json = JsonSerializer.Serialize(new { name = newAdapterName });
                    await Utils.RequestController(HttpMethod.Put, $"/proxies/{group.Name}", json);

                    this.RaisePropertyChanging(nameof(SelectedGroup));
                    group.Now = newAdapterName;
                    this.RaisePropertyChanged(nameof(SelectedGroup));
                });
            }
        }

        private async Task GetAdapters() {
            using HttpResponseMessage message = await Utils.RequestController(HttpMethod.Get, "/proxies");
            string json = await message.Content.ReadAsStringAsync();

            var result = JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, Adapter>>>(json);
            var adapters = result["proxies"];

            foreach ((string key, Adapter value) in adapters) {
                value.Name = key;
            }

            AllAdapters = adapters;
        }

        public void OnActivate() {
            Task.Run(GetAdapters);
        }
    }
}

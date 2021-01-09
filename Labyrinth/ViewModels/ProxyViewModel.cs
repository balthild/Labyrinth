using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reactive.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Labyrinth.Models;
using Labyrinth.Support;
using ReactiveUI;

namespace Labyrinth.ViewModels {
    public class ProxyViewModel : ViewModel {
        public class AdapterGroup {
            public Adapter Group { get; }

            public IEnumerable<Adapter> Adapters { get; }

            public Adapter? Current { get; set; }

            public AdapterGroup(Adapter group, IEnumerable<Adapter> adapters, Adapter? current = null) {
                Group = group;
                Adapters = adapters;
                Current = current;
            }
        }

        private Dictionary<string, Adapter> allAdapters = new();

        public Dictionary<string, Adapter> AllAdapters {
            get => allAdapters;
            set => this.RaiseAndSetIfChanged(ref allAdapters, value);
        }

        private readonly ObservableAsPropertyHelper<IEnumerable<AdapterGroup>> adapterGroups;

        public IEnumerable<AdapterGroup> AdapterGroups => adapterGroups.Value;

        public ProxyViewModel() {
            this.WhenAnyValue(x => x.AllAdapters, x => x.GlobalState.ClashConfig.Mode)
                .Select(tuple => {
                    var (all, mode) = tuple;
                    return GetGroups(all, mode).Select(group => new AdapterGroup(
                        group,
                        GetAdaptersInGroup(all, group),
                        all.GetValueOrDefault(group.Now, null)
                    ));
                })
                .ToProperty(this, nameof(AdapterGroups), out adapterGroups);

            Task.Run(GetAdapters);
        }

        private async Task GetAdapters() {
            var adapters = await DataUtils.GetAdapters();

            await SyncData(() => {
                AllAdapters = adapters;
            });
        }

        [UsedImplicitly]
        public void SwitchSelectorAdapter(object values) {
            if (values is not (AdapterGroup group, string newAdapterName))
                return;

            if (group.Group.Type != "Selector" || group.Group.Now == newAdapterName)
                return;

            Task.Run(async delegate {
                string json = JsonSerializer.Serialize(new { name = newAdapterName });
                await ApiController.Request(HttpMethod.Put, $"/proxies/{group.Group.Name}", json);

                AppConfig appConfig = await SyncData(() => GlobalState.AppConfig);
                await DataUtils.SaveSelectedAdaptersRecord(appConfig);
                await ConfigFile.SaveAppConfig(appConfig);

                // TODO: maybe we can update the data and view more efficiently?
                await GetAdapters();
            });
        }

        private static IEnumerable<Adapter> GetGroups(Dictionary<string, Adapter> all, string mode) {
            return all
                .Select(x => x.Value)
                .Where(x => x.Name switch {
                    // Only shows GLOBAL group under Global mode
                    "GLOBAL" => mode == "global",
                    _ => x.IsGroup(),
                })
                .OrderBy(x => x.Name switch {
                    "GLOBAL" => 1,
                    "Proxy" => 2,
                    _ => 99,
                });
        }

        private static IEnumerable<Adapter> GetAdaptersInGroup(Dictionary<string, Adapter> all, Adapter group) {
            return group.Name switch {
                "GLOBAL" => group.All
                    .Select(name => all.GetValueOrDefault(name)!)
                    .OrderBy(adapter => adapter.Type switch {
                        "Global" => 1,
                        "Direct" => 2,
                        "Reject" => 3,
                        "URLTest" => 10,
                        "Fallback" => 10,
                        "LoadBalance" => 10,
                        "Selector" => 10,
                        _ => 99,
                    }),
                _ => group.All.Select(name => all.GetValueOrDefault(name)!),
            };
        }
    }
}

using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Labyrinth.Models;
using Labyrinth.Support;
using ReactiveUI;

namespace Labyrinth.ViewModels {
    public class OverviewViewModel : ViewModelBase {
        private TrafficEntry currentTraffic = new TrafficEntry();

        public TrafficEntry CurrentTraffic {
            get => currentTraffic;
            set => this.RaiseAndSetIfChanged(ref currentTraffic, value);
        }

        private LinkedList<TrafficEntry> trafficsInternal = new LinkedList<TrafficEntry>(new TrafficEntry[60]);

        private ImmutableArray<TrafficEntry> traffics;

        public ImmutableArray<TrafficEntry> Traffics {
            get => traffics;
            set => this.RaiseAndSetIfChanged(ref traffics, value);
        }

        public OverviewViewModel() {
            Task.Factory.StartNew(UpdateTrafficLoop, TaskCreationOptions.LongRunning);
        }

        private async Task UpdateTrafficLoop() {
            while (true) {
                try {
                    await using Stream stream = await ApiController.PollStream("/traffic");
                    using var reader = new StreamReader(stream);

                    while (true) {
                        string? line = await reader.ReadLineAsync();
                        if (line == null)
                            continue;

                        var traffic = JsonSerializer.Deserialize<TrafficEntry>(line);

                        lock (trafficsInternal) {
                            trafficsInternal.RemoveFirst();
                            trafficsInternal.AddLast(traffic);
                            Traffics = trafficsInternal.ToImmutableArray();

                            CurrentTraffic = traffic;
                        }
                    }
                } catch {
                    await Task.Delay(1000);
                }
            }
        }

        public void ChangeMode(string mode) {
            Task.Run(async delegate {
                string body = JsonSerializer.Serialize(new { mode });
                await ApiController.Request(HttpMethod.Patch, "/configs", body);
            });

            GlobalState.RaisePropertyChanging(nameof(GlobalState.ClashConfig));
            GlobalState.ClashConfig.Mode = mode;
            GlobalState.RaisePropertyChanged(nameof(GlobalState.ClashConfig));
        }
    }
}

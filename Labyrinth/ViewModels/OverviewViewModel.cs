using System;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Labyrinth.Models;
using Labyrinth.Support;
using ReactiveUI;

namespace Labyrinth.ViewModels {
    public class OverviewViewModel : ViewModelBase, ITabContentViewModel {
        private string currentMode = "Rule";

        public string CurrentMode {
            get => currentMode;
            set => this.RaiseAndSetIfChanged(ref currentMode, value);
        }

        public OverviewViewModel() {
            Task.Run(UpdateTrafficLoop);
        }

        private async Task UpdateTrafficLoop() {
            while (true) {
                try {
                    HttpResponseMessage message = await Utils.RequestController(HttpMethod.Get,  "/traffic");
                    await using Stream stream = await message.Content.ReadAsStreamAsync();
                    using var reader = new StreamReader(stream);

                    while (true) {
                        string line = await reader.ReadLineAsync();
                        if (line == null)
                            continue;

                        var traffic = JsonSerializer.Deserialize<TrafficEntry>(line);
                        // TODO
                    }
                } catch (Exception e) {
                    Console.WriteLine(e);
                    await Task.Delay(1000);
                    // ignored
                }
            }
        }

        private async Task GetCurrentMode() {
            using HttpResponseMessage message = await Utils.RequestController(HttpMethod.Get, "/configs");
            string json = await message.Content.ReadAsStringAsync();
            var config = JsonSerializer.Deserialize<ClashConfig>(json);
            CurrentMode = config.Mode;
        }

        public void ChangeMode(string mode) {
            Task.Run(async delegate {
                string body = JsonSerializer.Serialize(new { mode });
                await Utils.RequestController(HttpMethod.Patch, "/configs", body);
            });

            CurrentMode = mode;
        }

        public void OnActivate() {
            Task.Run(GetCurrentMode);
        }
    }
}

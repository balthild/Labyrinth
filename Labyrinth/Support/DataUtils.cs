using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Labyrinth.Models;
using Labyrinth.ViewModels;
using ReactiveUI;

namespace Labyrinth.Support {
    public static class DataUtils {
        public static async Task<Dictionary<string, Adapter>> GetAdapters() {
            using HttpResponseMessage message = await ApiController.Request(HttpMethod.Get, "/proxies");
            byte[] json = await message.Content.ReadAsByteArrayAsync();

            var result = JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, Adapter>>>(json);
            var adapters = result["proxies"];

            foreach ((string key, Adapter value) in adapters) {
                value.Name = key;
            }

            return adapters;
        }

        public static async Task SetLastSelectedAdapters(AppConfig config) {
            // Set adapters if the record exists
            if (config.SelectedAdapters.TryGetValue(config.ConfigFile, out var record)) {
                foreach ((string group, string now) in record) {
                    string json = JsonSerializer.Serialize(new { name = now });
                    await ApiController.Request(HttpMethod.Put, $"/proxies/{group}", json);
                }
            }

            // Save adapters record anyway
            await SaveSelectedAdaptersRecord(config);
        }

        public static async Task SaveSelectedAdaptersRecord(AppConfig config) {
            Dictionary<string, string> record = new();
            foreach ((string name, Adapter adapter) in await GetAdapters()) {
                if (adapter.IsGroup()) {
                    record[name] = adapter.Now;
                }
            }

            await ViewModel.SyncData(() => {
                ViewModel.StaticState.RaisePropertyChanging(nameof(ViewModel.StaticState.AppConfig));
                config.SelectedAdapters[config.ConfigFile] = record;
                ViewModel.StaticState.RaisePropertyChanged(nameof(ViewModel.StaticState.AppConfig));
            });
        }

        public static async Task<ClashConfig> RefreshClashConfig() {
            using HttpResponseMessage message = await ApiController.Request(HttpMethod.Get, "/configs");
            string json = await message.Content.ReadAsStringAsync();
            var config = JsonSerializer.Deserialize<ClashConfig>(json);

            await ViewModel.SyncData(() => {
                ViewModel.StaticState.ClashConfig = config;
            });

            return config;
        }
    }
}

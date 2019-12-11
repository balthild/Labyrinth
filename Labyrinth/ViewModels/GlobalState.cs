using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Labyrinth.Models;
using Labyrinth.Support;
using ReactiveUI;

namespace Labyrinth.ViewModels {
    public class GlobalState : ReactiveObject {
        private bool running = false;

        public bool Running {
            get => running;
            set => this.RaiseAndSetIfChanged(ref running, value);
        }

        private AppConfig appConfig = new AppConfig();

        public AppConfig AppConfig {
            get => appConfig;
            set => this.RaiseAndSetIfChanged(ref appConfig, value);
        }

        private ClashConfig clashConfig = new ClashConfig();

        public ClashConfig ClashConfig {
            get => clashConfig;
            set => this.RaiseAndSetIfChanged(ref clashConfig, value);
        }

        private ClashController clashController = new ClashController();

        public ClashController ClashController {
            get => clashController;
            set => this.RaiseAndSetIfChanged(ref clashController, value);
        }

        public async Task RefreshClashConfig() {
            using HttpResponseMessage message = await ApiController.Request(HttpMethod.Get, "/configs");
            string json = await message.Content.ReadAsStringAsync();
            ClashConfig = JsonSerializer.Deserialize<ClashConfig>(json);
        }
    }
}

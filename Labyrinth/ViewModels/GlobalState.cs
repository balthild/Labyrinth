using Labyrinth.Models;
using ReactiveUI;

namespace Labyrinth.ViewModels {
    public class GlobalState : ReactiveObject {
        private AppConfig appConfig = new();

        public AppConfig AppConfig {
            get => appConfig;
            set => this.RaiseAndSetIfChanged(ref appConfig, value);
        }

        private ClashConfig clashConfig = new();

        public ClashConfig ClashConfig {
            get => clashConfig;
            set => this.RaiseAndSetIfChanged(ref clashConfig, value);
        }

        private ClashController clashController = new();

        public ClashController ClashController {
            get => clashController;
            set => this.RaiseAndSetIfChanged(ref clashController, value);
        }
    }
}

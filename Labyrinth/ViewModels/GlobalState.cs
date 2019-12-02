using Labyrinth.Models;
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

        private ClashController clashController = new ClashController();

        public ClashController ClashController {
            get => clashController;
            set => this.RaiseAndSetIfChanged(ref clashController, value);
        }
    }
}

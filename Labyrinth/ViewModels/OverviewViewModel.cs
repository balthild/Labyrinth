using ReactiveUI;

namespace Labyrinth.ViewModels {
    public class OverviewViewModel : ViewModelBase {
        private string currentMode = "Rule";

        public string CurrentMode {
            get => currentMode;
            set => this.RaiseAndSetIfChanged(ref currentMode, value);
        }

        public void ChangeMode(string mode) {
            CurrentMode = mode;
        }
    }
}

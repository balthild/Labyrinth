using ReactiveUI;

namespace Labyrinth.ViewModels {
    public class MainWindowViewModel : ViewModel {
        private ViewModel? currentWindowContent;

        public ViewModel? CurrentWindowContent {
            get => currentWindowContent;
            set => this.RaiseAndSetIfChanged(ref currentWindowContent, value);
        }
    }
}

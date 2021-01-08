using ReactiveUI;

namespace Labyrinth.ViewModels {
    public class InitializationViewModel : ViewModel {
        private long progressMax;

        public long ProgressMax {
            get => progressMax;
            set => this.RaiseAndSetIfChanged(ref progressMax, value);
        }

        private long progressValue;

        public long ProgressValue {
            get => progressValue;
            set => this.RaiseAndSetIfChanged(ref progressValue, value);
        }

        private string message = "Overview";

        public string Message {
            get => message;
            set => this.RaiseAndSetIfChanged(ref message, value);
        }
    }
}

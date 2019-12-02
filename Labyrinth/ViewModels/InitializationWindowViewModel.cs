using ReactiveUI;

namespace Labyrinth.ViewModels {
    public class InitializationWindowViewModel : ViewModelBase {
        private long totalBytesToReceive = 0;

        public long TotalBytesToReceive {
            get => totalBytesToReceive;
            set => this.RaiseAndSetIfChanged(ref totalBytesToReceive, value);
        }

        private long bytesReceived = 0;

        public long BytesReceived {
            get => bytesReceived;
            set => this.RaiseAndSetIfChanged(ref bytesReceived, value);
        }
    }
}

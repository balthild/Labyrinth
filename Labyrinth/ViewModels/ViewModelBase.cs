using ReactiveUI;

namespace Labyrinth.ViewModels {
    public abstract class ViewModelBase : ReactiveObject {
        public static readonly GlobalState State = new GlobalState();

        public GlobalState GlobalState => State;
    }
}

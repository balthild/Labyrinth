using System;
using System.Threading.Tasks;
using Avalonia.Threading;
using ReactiveUI;

namespace Labyrinth.ViewModels {
    public abstract class ViewModel : ReactiveObject {
        public static readonly GlobalState StaticState = new();

        // ReSharper disable once MemberCanBeMadeStatic.Global
        public GlobalState GlobalState => StaticState;

        public static Task<T> SyncData<T>(Func<T> f) => Dispatcher.UIThread.InvokeAsync(f);

        public static Task SyncData(Action f) => Dispatcher.UIThread.InvokeAsync(f);
    }
}

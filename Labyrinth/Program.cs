using Avalonia;
using Avalonia.Controls;
using Avalonia.Logging.Serilog;
using Avalonia.ReactiveUI;

namespace Labyrinth {
    class Program {
        public static void Main(string[] args) =>
            BuildAvaloniaApp().StartWithClassicDesktopLifetime(args, ShutdownMode.OnExplicitShutdown);

        public static AppBuilder BuildAvaloniaApp() =>
            AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .LogToDebug()
                .UseReactiveUI();
    }
}

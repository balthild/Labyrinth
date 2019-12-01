using System;
using System.IO;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using Labyrinth.Support;
using Labyrinth.Support.Interop;
using Labyrinth.ViewModels;
using Labyrinth.Views;

namespace Labyrinth {
    public class App : Application {
        public override void Initialize() {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted() {
            base.OnFrameworkInitializationCompleted();
            Task.Run(RunApp).Wait();
        }

        private async Task RunApp() {
            await EnsureConfigDir();
            Dispatcher.UIThread.Post(CreateMainWindow);
        }

        private void CreateMainWindow() {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop) {
                desktop.MainWindow = new MainWindow {
                    DataContext = new MainWindowViewModel()
                };
                desktop.MainWindow.Show();
            }
        }

        private async Task EnsureConfigDir() {
            Directory.CreateDirectory(Clash.ConfigDir);

            await EnsureMainConfigFile();

            if (!Clash.IsMaxmindDatabaseOk()) {
                Console.WriteLine("MMDB NOT OK");
            }
        }

        private async Task EnsureMainConfigFile() {
            string configPath = Clash.ConfigDir + "/config.yaml";
            string configPathOld = Clash.ConfigDir + "/config.yml";

            if (!File.Exists(configPath)) {
                if (File.Exists(configPathOld)) {
                    File.Move(configPathOld, configPath);
                } else {
                    await ConfigFile.ExtractDefaultClashConfig(configPath);
                    return;
                }
            }

            if (new FileInfo(configPath).Length == 0) {
                await ConfigFile.ExtractDefaultClashConfig(configPath);
            }
        }
    }
}

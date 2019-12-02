using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using ICSharpCode.SharpZipLib.GZip;
using ICSharpCode.SharpZipLib.Tar;
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
            Task.Run(RunApp);
        }

        private async Task RunApp() {
            await EnsureConfigDir();

            Dispatcher.UIThread.Post(delegate {
                if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop) {
                    desktop.MainWindow = new MainWindow { DataContext = new MainWindowViewModel() };
                    desktop.MainWindow.Show();
                }
            });
        }

        private async Task EnsureConfigDir() {
            Directory.CreateDirectory(Clash.ConfigDir);

            await EnsureMainConfigFile();

            if (!Clash.IsMaxmindDatabaseOk()) {
                await DownloadMaxmindDatabase();
            }
        }

        private async Task EnsureMainConfigFile() {
            string configPath = Path.Combine(Clash.ConfigDir, "config.yaml");
            string configPathOld = Path.Combine(Clash.ConfigDir, "config.yml");

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

        private async Task DownloadMaxmindDatabase() {
            var vm = new InitializationWindowViewModel();
            Dispatcher.UIThread.Post(delegate {
                if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop) {
                    desktop.MainWindow = new InitializationWindow { DataContext = vm };
                    desktop.MainWindow.Show();
                }
            });

            var client = new WebClient();
            client.DownloadProgressChanged += delegate(object sender, DownloadProgressChangedEventArgs args) {
                vm.BytesReceived = args.BytesReceived;
                vm.TotalBytesToReceive = args.TotalBytesToReceive;
            };

            const string url = "http://geolite.maxmind.com/download/geoip/database/GeoLite2-Country.tar.gz";
            var data = await client.DownloadDataTaskAsync(url);

            await using var dataStream = new MemoryStream(data, false);
            await using var gzipStream = new GZipInputStream(dataStream);
            await using var tarStream = new TarInputStream(gzipStream);

            TarEntry tarEntry;
            while ((tarEntry = tarStream.GetNextEntry()) != null) {
                if (!tarEntry.Name.EndsWith("GeoLite2-Country.mmdb"))
                    continue;

                string dbPath = Path.Combine(Clash.ConfigDir, "Country.mmdb");
                await using FileStream dbFileStream = File.OpenWrite(dbPath);
                await tarStream.CopyToAsync(dbFileStream);
                break;
            }

            Dispatcher.UIThread.Post(delegate {
                if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop) {
                    desktop.MainWindow.Close();
                    desktop.MainWindow = null;
                }
            });
        }
    }
}

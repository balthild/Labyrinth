using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using ICSharpCode.SharpZipLib.GZip;
using ICSharpCode.SharpZipLib.Tar;
using Labyrinth.Models;
using Labyrinth.Support;
using Labyrinth.Support.Interop;
using Labyrinth.ViewModels;
using Labyrinth.Views;
using ReactiveUI;

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
            await StartClash();

            Dispatcher.UIThread.Post(delegate {
                if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop) {
                    desktop.MainWindow = new MainWindow { DataContext = new MainWindowViewModel() };
                    desktop.MainWindow.Show();
                }
            });
        }

        private async Task EnsureConfigDir() {
            Directory.CreateDirectory(Clash.ConfigDir);

            if (!Clash.IsMaxmindDatabaseOk()) {
                await DownloadMaxmindDatabase();
            }

            await EnsureMainClashConfig();
            await EnsureAppConfig();
        }

        private async Task EnsureMainClashConfig() {
            string configPath = ConfigFile.GetPath("config.yaml");
            string configPathOld = ConfigFile.GetPath("config.yml");

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

        private async Task EnsureAppConfig() {
            string path = ConfigFile.GetPath("labyrinth.json");

            try {
                await using FileStream file = File.OpenRead(path);
                ViewModelBase.State.AppConfig = await JsonSerializer.DeserializeAsync<AppConfig>(file);
            } catch {
                // ViewModelBase.State.AppConfig remains default
            }

            // Write absent default values back
            await ConfigFile.SaveCurrentAppConfig();
        }

        private async Task StartClash() {
            Clash.StartResult result = Clash.Start();

            if (result.code == 0) {
                ViewModelBase.State.Running = true;
                ViewModelBase.State.ClashController = new ClashController {
                    Address = result.addr,
                    Secret = result.secret,
                };
                Utils.UpdateControllerClient(ViewModelBase.State.ClashController);
            }

            if (ConfigFile.GetClashConfigs().Contains(ViewModelBase.State.AppConfig.ConfigFile)) {
                string path = ConfigFile.GetPath(ViewModelBase.State.AppConfig.ConfigFile);
                string json = JsonSerializer.Serialize(new { path });
                await Utils.RequestController(HttpMethod.Put, "/configs", json);
            } else {
                ViewModelBase.State.AppConfig.ConfigFile = "config.yaml";
                ViewModelBase.State.RaisePropertyChanged(nameof(ViewModelBase.State.AppConfig));
                await ConfigFile.SaveCurrentAppConfig();
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

            using var client = new WebClient();
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

                string dbPath = ConfigFile.GetPath("Country.mmdb");
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

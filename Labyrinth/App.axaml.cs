using System;
using System.Collections.Generic;
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
using Labyrinth.Models;
using Labyrinth.Support;
using Labyrinth.Support.Interop;
using Labyrinth.ViewModels;
using Labyrinth.Views;
using ReactiveUI;

namespace Labyrinth {
    public class App : Application {
        private readonly MainWindowViewModel model = new() {
            CurrentWindowContent = new InitializationViewModel {
                Message = "Starting Labyrinth...",
            },
        };

        public override void Initialize() {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted() {
            base.OnFrameworkInitializationCompleted();

            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop) {
                desktop.MainWindow = new MainWindow {
                    DataContext = model,
                };
                desktop.MainWindow.Show();
            }

            Task.Run(RunApp);
        }

        private async Task RunApp() {
            try {
                await InitializeConfigDir();
                await InitializeMainClashConfig();
                await InitializeAppConfig();
                await StartClash();
                await SetClashConfigFile();
                await SetLastSelectedProxies();

                await DataUtils.RefreshClashConfig();

                AppConfig config = await ViewModel.SyncData(() => ViewModel.StaticState.AppConfig);
                await ConfigFile.SaveAppConfig(config);

                Dispatcher.UIThread.Post(() => model.CurrentWindowContent = new MainViewModel());
            } catch (Exception e) {
                Console.WriteLine(e);

                Dispatcher.UIThread.Post(() => {
                    var dialog = new MessageDialog($"{e.GetType()}:\n\n{e.Message}", "Failed to start Labyrinth");
                    dialog.Closed += delegate { Exit(); };

                    var desktop = Current.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime;
                    dialog.ShowDialog(desktop?.MainWindow);
                });
            }
        }

        private async Task InitializeConfigDir() {
            Directory.CreateDirectory(Clash.ConfigDir);

            if (!Clash.IsMaxmindDatabaseOk()) {
                await DownloadMaxmindDatabase();
            }

            Dispatcher.UIThread.Post(() => {
                model.CurrentWindowContent = new InitializationViewModel {
                    Message = "Starting Clash...",
                };
            });
        }

        private async Task DownloadMaxmindDatabase() {
            var vmProgress = new InitializationViewModel {
                Message = "Downloading MMDB...",
            };

            Dispatcher.UIThread.Post(() => {
                model.CurrentWindowContent = vmProgress;
            });

            using var client = new WebClient();
            client.DownloadProgressChanged += delegate(object _, DownloadProgressChangedEventArgs args) {
                vmProgress.ProgressValue = args.BytesReceived;
                vmProgress.ProgressMax = args.TotalBytesToReceive;
            };

            const string url = "http://geolite.maxmind.com/download/geoip/database/GeoLite2-Country.tar.gz";
            byte[] data = await client.DownloadDataTaskAsync(url);

            string path = ConfigFile.GetPath("Country.mmdb");
            await using FileStream file = File.OpenWrite(path);
            await file.WriteAsync(data);
        }

        private static async Task InitializeMainClashConfig() {
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

        private static async Task InitializeAppConfig() {
            string path = ConfigFile.GetPath("Labyrinth.json");

            AppConfig config;
            try {
                await using FileStream file = File.OpenRead(path);
                config = await JsonSerializer.DeserializeAsync<AppConfig>(file);

                await ViewModel.SyncData(() => {
                    ViewModel.StaticState.AppConfig = config;
                });
            } catch (Exception e) {
                Console.WriteLine(e);
                config = new AppConfig();
            }

            // Write absent default values back
            await ConfigFile.SaveAppConfig(config);
        }

        private static async Task StartClash() {
            Clash.StartResult result = Clash.Start();

            if (result.Code != 0) {
                string message = result.Code switch {
                    1 => "Failed to read or parse config file. Please check the config.yaml file and its content.",
                    2 => "The address of external controller is invalid. " +
                         "Please check the the option value in config.yaml.",
                    3 => "The port of external controller is unavailable. " +
                         "Please specify another port in config.yaml or stop programs that occupies the same port.",
                    _ => throw new ArgumentOutOfRangeException(),
                };

                throw new Exception(message);
            }

            var controller = new ClashController {
                Address = result.Addr!,
                Secret = result.Secret!,
            };

            await ViewModel.SyncData(() => {
                ViewModel.StaticState.ClashController = controller;
            });

            ApiController.UpdateClient(controller);
        }

        private static async Task SetClashConfigFile() {
            // Check if the last selected config exists in the filesystem
            if (ConfigFile.GetClashConfigs().Contains(ViewModel.StaticState.AppConfig.ConfigFile)) {
                // Switch to the selected config
                string path = ConfigFile.GetPath(ViewModel.StaticState.AppConfig.ConfigFile);
                string json = JsonSerializer.Serialize(new { path });
                await ApiController.Request(HttpMethod.Put, "/configs", json);
            } else {
                // Reset the selected config to config.yaml
                await ViewModel.SyncData(() => {
                    ViewModel.StaticState.RaisePropertyChanging(nameof(ViewModel.StaticState.AppConfig));
                    ViewModel.StaticState.AppConfig.ConfigFile = "config.yaml";
                    ViewModel.StaticState.RaisePropertyChanged(nameof(ViewModel.StaticState.AppConfig));
                });
            }
        }

        private static async Task SetLastSelectedProxies() {
            AppConfig appConfig = await ViewModel.SyncData(() => ViewModel.StaticState.AppConfig);

            // Set Mode
            string body = JsonSerializer.Serialize(new { mode = appConfig.Mode });
            await ApiController.Request(HttpMethod.Patch, "/configs", body);

            await DataUtils.SetLastSelectedAdapters(appConfig);
        }

        public static void Exit() {
            IApplicationLifetime lifetime = Current.ApplicationLifetime;

            if (lifetime is IClassicDesktopStyleApplicationLifetime desktop) {
                if (desktop.MainWindow is MainWindow window)
                    window.Exit();
                else
                    desktop.MainWindow.Close();

                desktop.Shutdown();
            }
        }
    }
}

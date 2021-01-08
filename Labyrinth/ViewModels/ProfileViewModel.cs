using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reactive;
using System.Reactive.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Threading;
using JetBrains.Annotations;
using Labyrinth.Models;
using Labyrinth.Support;
using Labyrinth.Support.Interop;
using Labyrinth.Views;
using ReactiveUI;

namespace Labyrinth.ViewModels {
    public class ProfileViewModel : ViewModel {
        private IEnumerable<Profile> profiles = Enumerable.Empty<Profile>();

        public IEnumerable<Profile> Profiles {
            get => profiles;
            set => this.RaiseAndSetIfChanged(ref profiles, value);
        }

        private Profile? selectedProfile;

        public Profile? SelectedProfile {
            get => selectedProfile;
            set => this.RaiseAndSetIfChanged(ref selectedProfile, value);
        }

        private string activeProfileName = "config.yaml";

        public string ActiveProfileName {
            get => activeProfileName;
            set => this.RaiseAndSetIfChanged(ref activeProfileName, value);
        }

        public ReactiveCommand<Profile, Unit> UpdateSelectedSubscription { get; }

        public ProfileViewModel() {
            var isSubscription = this.WhenAnyValue(x => x.SelectedProfile).Select(x => x?.Subscription != null);
            UpdateSelectedSubscription = ReactiveCommand.CreateFromTask<Profile>(TryUpdateSubscription, isSubscription);

            GetProfilesFromFilesystem();
        }

        private void GetProfilesFromFilesystem() {
            string[] configs = ConfigFile.GetClashConfigs().ToArray();

            Dispatcher.UIThread.Post(() => {
                Profiles = configs.Select(name => {
                    Subscription? subscription = GlobalState.AppConfig.Subscriptions.GetValueOrDefault(name);
                    return new Profile { Name = name, Subscription = subscription };
                });

                ActiveProfileName = configs.First(x => x == GlobalState.AppConfig.ConfigFile) ?? "config.yaml";
            });
        }

        public async Task NewSubscription() {
            var desktop = Application.Current.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime;
            var dialog = new NewSubscriptionDialog();
            var result = await dialog.ShowDialog<NewSubscriptionDialog.Result?>(desktop?.MainWindow);
            if (result == null)
                return;

            await File.WriteAllBytesAsync(ConfigFile.GetPath(result.Name), result.Data);

            AppConfig config = await SyncData(() => {
                GlobalState.RaisePropertyChanging(nameof(GlobalState.AppConfig));
                GlobalState.AppConfig.Subscriptions[result.Name] = new Subscription {
                    Url = result.Url,
                    UpdatedAt = DateTimeOffset.Now.ToUnixTimeSeconds(),
                };
                GlobalState.RaisePropertyChanged(nameof(GlobalState.AppConfig));

                return GlobalState.AppConfig;
            });

            await ConfigFile.SaveAppConfig(config);

            GetProfilesFromFilesystem();
        }

        private async Task TryUpdateSubscription(Profile profile) {
            try {
                await UpdateSubscription(profile);
            } catch (Exception e) {
                Console.WriteLine(e);

                Dispatcher.UIThread.Post(() => {
                    var desktop = Application.Current.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime;
                    var dialog = new MessageDialog($"{e.GetType()}: {e.Message}", "Failed to update subscription");
                    dialog.ShowDialog(desktop?.MainWindow);
                });
            }
        }

        private async Task UpdateSubscription(Profile profile) {
            if (profile.Subscription == null)
                return;

            byte[] data = await Utils.HttpClient.GetByteArrayAsync(profile.Subscription.Url);

            string? error = Clash.ValidateConfig(data);
            if (error != null) {
                throw new Exception(error);
            }

            await File.WriteAllBytesAsync(ConfigFile.GetPath(profile.Name), data);

            AppConfig appConfig = await SyncData(() => {
                GlobalState.RaisePropertyChanging(nameof(GlobalState.AppConfig));
                profile.Subscription.UpdatedAt = DateTimeOffset.Now.ToUnixTimeSeconds();
                GlobalState.RaisePropertyChanged(nameof(GlobalState.AppConfig));

                return GlobalState.AppConfig;
            });

            await ConfigFile.SaveAppConfig(appConfig);

            if (ActiveProfileName == profile.Name) {
                await ApplyClashConfig(profile.Name);
                await DataUtils.SetLastSelectedAdapters(appConfig);
                await ConfigFile.SaveAppConfig(appConfig);
            }

            GetProfilesFromFilesystem();
        }

        [UsedImplicitly]
        public void SwitchProfile(string name) {
            if (ActiveProfileName == name)
                return;

            Task.Run(async delegate {
                await ApplyClashConfig(name);

                AppConfig appConfig = await SyncData(() => {
                    ActiveProfileName = name;

                    GlobalState.RaisePropertyChanging(nameof(GlobalState.AppConfig));
                    GlobalState.AppConfig.ConfigFile = name;
                    GlobalState.RaisePropertyChanged(nameof(GlobalState.AppConfig));

                    return GlobalState.AppConfig;
                });

                await DataUtils.SetLastSelectedAdapters(appConfig);
                await ConfigFile.SaveAppConfig(appConfig);
            });
        }

        public void SelectProfileFromList(SelectionChangedEventArgs args) {
            SelectedProfile = args.AddedItems.Cast<Profile>().FirstOrDefault();
        }

        private static async Task ApplyClashConfig(string name) {
            string json = JsonSerializer.Serialize(new { path = ConfigFile.GetPath(name) });
            await ApiController.Request(HttpMethod.Put, "/configs", json);

            await DataUtils.RefreshClashConfig();
        }
    }
}

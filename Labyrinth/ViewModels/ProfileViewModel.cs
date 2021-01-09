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

        private readonly ObservableAsPropertyHelper<string> activeProfileName;

        public string ActiveProfileName => activeProfileName.Value;

        public ReactiveCommand<Profile, Unit> UpdateSubscriptionCommand { get; }
        public ReactiveCommand<Profile, Unit> DeleteProfileCommand { get; }
        public ReactiveCommand<Unit, Unit> UpdateAllSubscriptionCommand { get; }

        public ProfileViewModel() {
            this.WhenAnyValue(x => x.GlobalState.AppConfig.ConfigFile)
                .ToProperty(this, nameof(ActiveProfileName), out activeProfileName);

            UpdateSubscriptionCommand = ReactiveCommand.CreateFromTask<Profile>(
                TryUpdateSubscription,
                this.WhenAnyValue(x => x.SelectedProfile).Select(x => x?.Subscription != null)
            );

            DeleteProfileCommand = ReactiveCommand.CreateFromTask<Profile>(
                DeleteProfile,
                this.WhenAnyValue(x => x.SelectedProfile, x => x.ActiveProfileName)
                    .Select(tuple => {
                        (Profile? selected, string active) = tuple;
                        return selected != null && selected.Name != "config.yaml" && selected.Name != active;
                    })
            );

            UpdateAllSubscriptionCommand = ReactiveCommand.CreateFromTask(TryUpdateAllSubscription);

            Task.Run(GetProfilesFromFilesystem);
        }

        private async Task GetProfilesFromFilesystem() {
            var subscriptions = await SyncData(() => GlobalState.AppConfig.Subscriptions);

            var profileTasks = ConfigFile.GetClashConfigs()
                .Select(async name => {
                    Subscription? subscription = subscriptions.GetValueOrDefault(name);
                    var stats = await ConfigFile.GetClashConfigFileStats(name);
                    return new Profile {
                        Name = name,
                        Subscription = subscription,
                        Stats = stats,
                    };
                })
                .ToArray();

            var allProfiles = await Task.WhenAll(profileTasks);

            await SyncData(() => { Profiles = allProfiles; });
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

            await GetProfilesFromFilesystem();
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

            byte[] data = await Utils.GetProxiedHttpClient().GetByteArrayAsync(profile.Subscription.Url);

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

            await GetProfilesFromFilesystem();
        }

        private async Task DeleteProfile(Profile profile) {
            // Delete file asynchronously.
            // See https://stackoverflow.com/questions/10606328/why-isnt-there-an-asynchronous-file-delete-in-net
            // TODO: shows a confirmation dialog
            await using var stream = new FileStream(
                ConfigFile.GetPath(profile.Name),
                FileMode.Open,
                FileAccess.Read,
                FileShare.None,
                4096,
                FileOptions.DeleteOnClose
            );
            await stream.FlushAsync();
            await stream.DisposeAsync();

            await GetProfilesFromFilesystem();
        }

        private async Task TryUpdateAllSubscription() {
            List<string> errored = new();

            foreach (Profile profile in Profiles) {
                try {
                    await UpdateSubscription(profile);
                } catch (Exception e) {
                    Console.WriteLine(e);
                    errored.Add(profile.Name);
                }
            }

            if (errored.Count > 0) {
                string filenames = string.Join("\n\n", errored);
                string message = $"Some error occurred when updating the following subscriptions:\n\n{filenames}";

                Dispatcher.UIThread.Post(() => {
                    var desktop = Application.Current.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime;
                    var dialog = new MessageDialog(message, "Failed to update subscription");
                    dialog.ShowDialog(desktop?.MainWindow);
                });
            }

            await GetProfilesFromFilesystem();
        }

        [UsedImplicitly]
        public Task SwitchProfile(string name) {
            if (ActiveProfileName == name)
                return Task.CompletedTask;

            return Task.Run(async delegate {
                await ApplyClashConfig(name);

                AppConfig appConfig = await SyncData(() => {
                    GlobalState.RaisePropertyChanging(nameof(GlobalState.AppConfig));
                    GlobalState.AppConfig.ConfigFile = name;
                    GlobalState.RaisePropertyChanged(nameof(GlobalState.AppConfig));

                    return GlobalState.AppConfig;
                });

                await DataUtils.SetLastSelectedAdapters(appConfig);
                await ConfigFile.SaveAppConfig(appConfig);
            });
        }

        private static async Task ApplyClashConfig(string name) {
            string json = JsonSerializer.Serialize(new { path = ConfigFile.GetPath(name) });
            await ApiController.Request(HttpMethod.Put, "/configs", json);

            await DataUtils.RefreshClashConfig();
        }
    }
}

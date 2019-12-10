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
using Labyrinth.Models;
using Labyrinth.Support;
using Labyrinth.Support.Interop;
using Labyrinth.Views;
using ReactiveUI;

namespace Labyrinth.ViewModels {
    public class ProfileViewModel : ViewModelBase {
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
            UpdateSelectedSubscription = ReactiveCommand.CreateFromTask<Profile>(UpdateSubscription, isSubscription);

            GetProfiles();
        }

        private void GetProfiles() {
            var configs = ConfigFile.GetClashConfigs().ToArray();

            Profiles = configs.Select(name => new Profile {
                Name = name,
                Subscription = State.AppConfig.Subscriptions.GetValueOrDefault(name)
            });

            ActiveProfileName = configs.First(x => x == State.AppConfig.ConfigFile) ?? "config.yaml";
        }

        public async Task NewSubscription() {
            var desktop = Application.Current.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime;
            var dialog = new NewSubscriptionDialog();
            var result = await dialog.ShowDialog<object?>(desktop?.MainWindow);
            Console.WriteLine(result);
        }

        private async Task UpdateSubscription(Profile profile) {
            if (profile.Subscription == null)
                return;

            using var client = new HttpClient();
            var data = await client.GetByteArrayAsync(profile.Subscription.Url);

            string? error = Clash.ValidateConfig(data);
            if (error != null) {
                // Show alert
                Console.WriteLine(error);
                return;
            }

            await File.WriteAllBytesAsync(ConfigFile.GetPath(profile.Name), data);

            profile.Subscription.UpdatedAt = DateTimeOffset.Now.ToUnixTimeSeconds();

            State.RaisePropertyChanging(nameof(State.AppConfig));
            State.AppConfig.Subscriptions[profile.Name] = profile.Subscription;
            State.RaisePropertyChanged(nameof(State.AppConfig));

            await ApplyClashConfig(profile.Name);
            await ConfigFile.SaveCurrentAppConfig();

            GetProfiles();
        }

        public void SwitchProfile(string name) {
            if (ActiveProfileName == name)
                return;

            Task.Run(async delegate {
                await ApplyClashConfig(name);

                ActiveProfileName = name;
                State.AppConfig.ConfigFile = name;
                await ConfigFile.SaveCurrentAppConfig();
            });
        }

        private async Task ApplyClashConfig(string name) {
            string json = JsonSerializer.Serialize(new { path = ConfigFile.GetPath(name) });
            await Utils.RequestController(HttpMethod.Put, "/configs", json);
            await State.RefreshClashConfig();
        }

        public void SelectProfile(SelectionChangedEventArgs args) {
            SelectedProfile = args.AddedItems.Cast<Profile>().FirstOrDefault();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Labyrinth.Models;
using Labyrinth.Support;
using ReactiveUI;

namespace Labyrinth.ViewModels {
    public class ProfileViewModel : ViewModelBase, ITabContentViewModel {
        private IEnumerable<Profile> profiles = Enumerable.Empty<Profile>();

        public IEnumerable<Profile> Profiles {
            get => profiles;
            set => this.RaiseAndSetIfChanged(ref profiles, value);
        }

        private string activeProfileName = "config.yaml";

        public string ActiveProfileName {
            get => activeProfileName;
            set => this.RaiseAndSetIfChanged(ref activeProfileName, value);
        }

        public void SwitchProfile(string name) {
            if (ActiveProfileName == name)
                return;

            Task.Run(async delegate {
                string json = JsonSerializer.Serialize(new { path = ConfigFile.GetPath(name) });
                await Utils.RequestController(HttpMethod.Put, $"/configs", json);

                ActiveProfileName = name;
                State.AppConfig.ConfigFile = name;
                await ConfigFile.SaveCurrentAppConfig();
            });
        }

        private void GetProfiles() {
            var configs = ConfigFile.GetClashConfigs().ToArray();

            Profiles = configs.Select(name => new Profile {
                Name = name,
                Subscription = State.AppConfig.Subscriptions.GetValueOrDefault(name)
            });

            ActiveProfileName = configs.First(x => x == State.AppConfig.ConfigFile) ?? "config.yaml";
        }

        public void OnActivate() {
            GetProfiles();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Threading.Tasks;
using ReactiveUI;

namespace Labyrinth.ViewModels {
    public class MainWindowViewModel : ViewModelBase {
        private string currentTabName = "Overview";

        public string CurrentTabName {
            get => currentTabName;
            set => this.RaiseAndSetIfChanged(ref currentTabName, value);
        }

        private ObservableAsPropertyHelper<ViewModelBase> currentTabContent;

        public ViewModelBase CurrentTabContent => currentTabContent.Value;

        private ObservableAsPropertyHelper<string[]> tabs;

        public string[] Tabs => tabs.Value;

        private readonly ViewModelBase overview = new OverviewViewModel();

        private ViewModelBase GetTabContent(string tab) => tab switch {
            "Proxy" => new ProxyViewModel(),
            "Profile" => new ProfileViewModel(),
            _ => overview
        };

        public MainWindowViewModel() {
            this.WhenAnyValue(x => x.GlobalState.ClashConfig.Mode)
                .Select(mode => mode switch {
                    "Direct" => new[] { "Overview", "Profile", "Log", "Settings" },
                    _ => new[] { "Overview", "Proxy", "Profile", "Log", "Settings" }
                })
                .ToProperty(this, nameof(Tabs), out tabs);

            this.WhenAnyValue(x => x.CurrentTabName)
                .Select(GetTabContent)
                .ToProperty(this, nameof(CurrentTabContent), out currentTabContent);

            Task.Run(State.RefreshClashConfig);
        }
    }
}

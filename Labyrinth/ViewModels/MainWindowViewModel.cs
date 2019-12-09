using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Reactive.Linq;
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

        public IEnumerable<string> Tabs => new [] {
            "Overview", "Proxy", "Profile", "Log", "Settings"
        };

        private readonly ViewModelBase overview = new OverviewViewModel();

        private ViewModelBase GetTabContent(string tab) => tab switch {
            "Proxy" => new ProxyViewModel(),
            "Profile" => new ProfileViewModel(),
            _ => overview
        };

        public MainWindowViewModel() {
            this.WhenAnyValue(x => x.CurrentTabName)
                .Select(GetTabContent)
                .ToProperty(this, nameof(CurrentTabContent), out currentTabContent);
        }
    }
}

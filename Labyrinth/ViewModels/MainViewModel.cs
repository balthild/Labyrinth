using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Labyrinth.Support;
using ReactiveUI;

namespace Labyrinth.ViewModels {
    public class MainViewModel : ViewModel {
        private static readonly IEnumerable<string> AllTabs = new [] {
            "Overview", "Proxy", "Profile", "Rule", "Log", "Connection", "Setting",
        };

        private string currentTabName = "Overview";

        public string CurrentTabName {
            get => currentTabName;
            set => this.RaiseAndSetIfChanged(ref currentTabName, value);
        }

        private readonly ObservableAsPropertyHelper<ViewModel> currentTabContent;

        public ViewModel CurrentTabContent => currentTabContent.Value;

        private readonly ObservableAsPropertyHelper<IEnumerable<string>> tabs;

        public IEnumerable<string> Tabs => tabs.Value;

        private readonly ViewModel overview = new OverviewViewModel();

        private ViewModel GetTabContent(string tab) => tab switch {
            "Proxy" => new ProxyViewModel(),
            "Profile" => new ProfileViewModel(),
            _ => overview,
        };

        public MainViewModel() {
            this.WhenAnyValue(x => x.GlobalState.ClashConfig.Mode)
                .Select(mode => mode.ToLowerInvariant() switch {
                    "direct" => AllTabs.Where(x => x != "Proxy"),
                    _ => AllTabs,
                })
                .ToProperty(this, nameof(Tabs), out tabs);

            this.WhenAnyValue(x => x.CurrentTabName)
                .Select(GetTabContent)
                .ToProperty(this, nameof(CurrentTabContent), out currentTabContent);
        }
    }
}

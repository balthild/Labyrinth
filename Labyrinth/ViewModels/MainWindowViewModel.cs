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

        private readonly OrderedDictionary tabContents = new OrderedDictionary {
            ["Overview"] = new OverviewViewModel(),
            ["Proxy"] = new OverviewViewModel(),
            ["Profile"] = new OverviewViewModel(),
            ["Log"] = new OverviewViewModel(),
            ["Settings"] = new OverviewViewModel(),
        };

        public IEnumerable<string> TabItems => tabContents.Keys.Cast<string>();

        public MainWindowViewModel() {
            currentTabContent = this.WhenAnyValue(x => x.CurrentTabName)
                .Select(x => (ViewModelBase) tabContents[x])
                .ToProperty(this, nameof(CurrentTabContent));
        }
    }
}

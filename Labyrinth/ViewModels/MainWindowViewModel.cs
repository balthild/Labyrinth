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

        private ObservableAsPropertyHelper<ITabContentViewModel> currentTabContent;

        public ITabContentViewModel CurrentTabContent => currentTabContent.Value;

        private readonly OrderedDictionary tabContents = new OrderedDictionary {
            ["Overview"] = new OverviewViewModel(),
            ["Proxy"] = new ProxyViewModel(),
            // ["Profile"] = new OverviewViewModel(),
            // ["Log"] = new OverviewViewModel(),
            // ["Settings"] = new OverviewViewModel(),
        };

        public IEnumerable<string> TabItems => tabContents.Keys.Cast<string>();

        public MainWindowViewModel() {
            this.WhenAnyValue(x => x.CurrentTabName)
                .Select(x => (ITabContentViewModel) tabContents[x])
                .ToProperty(this, nameof(CurrentTabContent), out currentTabContent);

            this.WhenAnyValue(x => x.CurrentTabContent)
                .Buffer(2, 1)
                .Subscribe(x => x[0].OnDeactivate());

            this.WhenAnyValue(x => x.CurrentTabContent)
                .Subscribe(x => x.OnActivate());
        }
    }
}

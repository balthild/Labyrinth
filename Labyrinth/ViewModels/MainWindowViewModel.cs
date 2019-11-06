using System;
using System.Collections.Generic;
using System.Text;
using System.Reactive.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Xaml.Interactivity;
using ReactiveUI;
using Labyrinth.Models;

namespace Labyrinth.ViewModels {
    public class MainWindowViewModel : ViewModelBase {

        public string currentTabName = "Overview";
        public string CurrentTabName {
            get => currentTabName;
            set => this.RaiseAndSetIfChanged(ref currentTabName, value);
        }

        public ObservableAsPropertyHelper<string> len;
        public string Len => len.Value;

        public NavItem[] TabItems => new[] {
            new NavItem("Overview"),
            new NavItem("Proxy"),
            new NavItem("Profile"),
            new NavItem("Log"),
            new NavItem("Settings"),
        };

        public MainWindowViewModel() {
            len = this.WhenAnyValue(x => x.CurrentTabName)
                .Select(x => x.Length.ToString())
                .ToProperty(this, nameof(Len));
        }

        public void AddCurrentTabClass(object sender, AvaloniaPropertyChangedEventArgs args) {
            var behavior = args.Sender as Behavior;
            var tab = behavior?.AssociatedObject as Control;
            tab?.Classes.Add("current");
        }

        public void RemoveCurrentTabClass(object sender, AvaloniaPropertyChangedEventArgs args) {
            var behavior = args.Sender as Behavior;
            var tab = behavior?.AssociatedObject as Control;
            tab?.Classes.Remove("current");
        }
    }
}

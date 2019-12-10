using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Xaml.Interactivity;
using JetBrains.Annotations;
using Labyrinth.Models;
using Labyrinth.ViewModels;

namespace Labyrinth.Views {
    public class ProxyView : UserControl {
        public ProxyView() {
            InitializeComponent();
        }

        private void InitializeComponent() {
            AvaloniaXamlLoader.Load(this);
        }

        [UsedImplicitly]
        public void AddActiveAdapterClass(object target, AvaloniaPropertyChangedEventArgs args) {
            var behavior = (Behavior) args.Sender;
            var tab = (Control) behavior.AssociatedObject!;
            tab?.Classes.Add("active");
        }

        [UsedImplicitly]
        public void RemoveActiveAdapterClass(object target, AvaloniaPropertyChangedEventArgs args) {
            var behavior = (Behavior) args.Sender;
            var tab = (Control) behavior.AssociatedObject!;
            tab?.Classes.Remove("active");
        }

        [UsedImplicitly]
        public void SetActiveAdapterClass(object sender, EventArgs args) {
            var control = (Control) sender;
            var adapter = (Adapter) control.DataContext;
            var vm = (ProxyViewModel) DataContext;
            control.Classes.Set("active", adapter.Name == vm.ActiveAdapterInSelectedGroup?.Name);
        }

        [UsedImplicitly]
        public void AddSelectorGroupClass(object target, AvaloniaPropertyChangedEventArgs args) {
            var behavior = (Behavior) args.Sender;
            var tab = (Control) behavior.AssociatedObject!;
            tab?.Classes.Add("selector");
        }

        [UsedImplicitly]
        public void RemoveSelectorGroupClass(object target, AvaloniaPropertyChangedEventArgs args) {
            var behavior = (Behavior) args.Sender;
            var tab = (Control) behavior.AssociatedObject!;
            tab?.Classes.Remove("selector");
        }

        [UsedImplicitly]
        public void ClearSelection(object sender, SelectionChangedEventArgs args) {
            ((ListBox) sender).UnselectAll();
        }
    }
}

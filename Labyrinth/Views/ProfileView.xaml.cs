using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Xaml.Interactivity;
using JetBrains.Annotations;
using Labyrinth.Models;
using Labyrinth.ViewModels;

namespace Labyrinth.Views {
    public class ProfileView : UserControl {
        public ProfileView() {
            InitializeComponent();
        }

        private void InitializeComponent() {
            AvaloniaXamlLoader.Load(this);
        }

        [UsedImplicitly]
        public void AddActiveProfileClass(object target, AvaloniaPropertyChangedEventArgs args) {
            var behavior = (Behavior) args.Sender;
            var tab = (Control) behavior.AssociatedObject!;
            tab?.Classes.Add("active");
        }

        [UsedImplicitly]
        public void RemoveActiveProfileClass(object target, AvaloniaPropertyChangedEventArgs args) {
            var behavior = (Behavior) args.Sender;
            var tab = (Control) behavior.AssociatedObject!;
            tab?.Classes.Remove("active");
        }

        [UsedImplicitly]
        public void SetActiveProfileClass(object sender, EventArgs args) {
            var control = (Control) sender;
            var profile = (Profile) control.DataContext;
            var vm = (ProfileViewModel) DataContext;
            control.Classes.Set("active", profile.Name == vm.ActiveProfileName);
        }

        [UsedImplicitly]
        public void ShowCreateSubscriptionDialog(object sender, RoutedEventArgs args) {
            // TODO: Dialog
        }
    }
}

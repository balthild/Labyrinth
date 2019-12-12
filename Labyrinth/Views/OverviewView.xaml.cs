using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using JetBrains.Annotations;

namespace Labyrinth.Views {
    public class OverviewView : UserControl {
        public OverviewView() {
            InitializeComponent();
        }

        private void InitializeComponent() {
            AvaloniaXamlLoader.Load(this);
        }

        [UsedImplicitly]
        public void ModeChanged(object target, AvaloniaPropertyChangedEventArgs args) {
            var radio = this.FindControl<RadioButton>("Mode" + args.NewValue);
            radio.IsChecked = true;
        }

        [UsedImplicitly]
        private void ExitApp(object sender, RoutedEventArgs args) {
            App.Exit();
        }
    }
}

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
            string str = args.NewValue as string ?? "rule";
            string mode = char.ToUpper(str[0]) + str.Substring(1);
            var radio = this.FindControl<RadioButton>($"Mode{mode}");
            radio.IsChecked = true;
        }

        [UsedImplicitly]
        private void ExitApp(object sender, RoutedEventArgs args) {
            App.Exit();
        }
    }
}

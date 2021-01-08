using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Xaml.Interactivity;
using JetBrains.Annotations;

namespace Labyrinth.Views {
    public class MainView : UserControl {
        public MainView() {
            InitializeComponent();
        }

        private void InitializeComponent() {
            AvaloniaXamlLoader.Load(this);

            this.FindControl<Control>("Title").PointerPressed += (_, e) => {
                if ((e.Source as Control)?.Classes.Contains("drag") ?? false) {
                    (VisualRoot as Window)?.BeginMoveDrag(e);
                }
            };
        }

        [UsedImplicitly]
        public void CloseWindow(object sender, RoutedEventArgs args) {
            (VisualRoot as Window)?.Close();
        }
    }
}

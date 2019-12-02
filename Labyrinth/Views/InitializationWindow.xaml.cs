using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Labyrinth.Views {
    public class InitializationWindow : BorderlessWindow {
        public InitializationWindow() {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
        }

        private void InitializeComponent() {
            AvaloniaXamlLoader.Load(this);

            this.FindControl<Control>("Title").PointerPressed += (i, e) => {
                if (((Control) e.Source).Classes.Contains("drag")) {
                    BeginMoveDrag(e);
                }
            };
        }
    }
}

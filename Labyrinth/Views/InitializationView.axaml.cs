using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using JetBrains.Annotations;

namespace Labyrinth.Views {
    [UsedImplicitly]
    public class InitializationView : UserControl {
        public InitializationView() {
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
    }
}

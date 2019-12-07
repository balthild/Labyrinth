using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Labyrinth.Views {
    public class ProxyView : UserControl {
        public ProxyView() {
            InitializeComponent();
        }

        private void InitializeComponent() {
            AvaloniaXamlLoader.Load(this);
        }
    }
}

using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Xaml.Interactivity;
using JetBrains.Annotations;

namespace Labyrinth.Views {
    public class MainWindow : BorderlessWindow {
        public MainWindow() {
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

        [UsedImplicitly]
        public void AddCurrentTabClass(object target, AvaloniaPropertyChangedEventArgs args) {
            var behavior = (Behavior) args.Sender;
            var tab = (Control) behavior.AssociatedObject!;
            tab?.Classes.Add("current");
        }

        [UsedImplicitly]
        public void RemoveCurrentTabClass(object target, AvaloniaPropertyChangedEventArgs args) {
            var behavior = (Behavior) args.Sender;
            var tab = (Control) behavior.AssociatedObject!;
            tab?.Classes.Remove("current");
        }
    }
}

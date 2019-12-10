using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Labyrinth.Controls;

namespace Labyrinth.Views {
    public class NewSubscriptionDialog : BorderlessWindow {
        public NewSubscriptionDialog() {
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

        public void Ok(object sender, RoutedEventArgs args) {
            Console.WriteLine(args);
        }

        public void Cancel(object sender, RoutedEventArgs args) {
            Close(null);
        }
    }
}

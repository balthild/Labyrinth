using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using JetBrains.Annotations;
using Labyrinth.Controls;

namespace Labyrinth.Views {
    public class MessageDialog : BorderlessWindow {
        [UsedImplicitly]
        public MessageDialog() : this("No message") {
            // Required by XAML
        }

        public MessageDialog(string message, string title = "Notice") {
            HideTaskbarIcon();

            InitializeComponent();

            this.FindControl<TextBlock>("TitleText").Text = title;
            this.FindControl<TextBlock>("MessageText").Text = message;
        }

        private void InitializeComponent() {
            AvaloniaXamlLoader.Load(this);

            this.FindControl<Control>("Title").PointerPressed += (_, e) => {
                if ((e.Source as Control)?.Classes.Contains("drag") ?? false) {
                    BeginMoveDrag(e);
                }
            };
        }

        public void Ok(object sender, RoutedEventArgs args) {
            Close();
        }
    }
}

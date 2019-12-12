using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Labyrinth.Controls;

namespace Labyrinth.Views {
    public class MessageDialog : BorderlessWindow {
        public MessageDialog() : this("No message") { }

        public MessageDialog(string message, string title = "Notice") {
            InitializeComponent();

            this.FindControl<TextBlock>("TitleText").Text = title;
            this.FindControl<TextBlock>("MessageText").Text = message;
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
            Close();
        }
    }
}

using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Threading;
using Labyrinth.Controls;
using Labyrinth.Models;
using Labyrinth.Support;
using Labyrinth.Support.Interop;

namespace Labyrinth.Views {
    public class NewSubscriptionDialog : BorderlessWindow {
        public class Result {
            public readonly string Name;
            public readonly string Url;
            public readonly byte[] Data;

            public Result(string name, string url, byte[] data) {
                Name = name;
                Url = url;
                Data = data;
            }
        }

        private readonly TextBox url;
        private readonly TextBox name;
        private readonly TextBlock error;
        private readonly Button ok;
        private readonly Button cancel;

        public NewSubscriptionDialog() {
            InitializeComponent();

            url = this.FindControl<TextBox>("Url");
            name = this.FindControl<TextBox>("Name");
            error = this.FindControl<TextBlock>("Error");
            ok = this.FindControl<Button>("Ok");
            cancel = this.FindControl<Button>("Cancel");
        }

        private void InitializeComponent() {
            AvaloniaXamlLoader.Load(this);

            this.FindControl<Control>("Title").PointerPressed += (i, e) => {
                if (((Control) e.Source).Classes.Contains("drag")) {
                    BeginMoveDrag(e);
                }
            };
        }

        private void DisableControls() {
            url.IsEnabled = false;
            url.Foreground = Brushes.Gray;

            name.IsEnabled = false;
            name.Foreground = Brushes.Gray;

            ok.IsEnabled = false;
            cancel.IsEnabled = false;
        }

        private void EnableControls() {
            url.IsEnabled = true;
            url.Foreground = null;

            name.IsEnabled = true;
            name.Foreground = null;

            ok.IsEnabled = true;
            cancel.IsEnabled = true;
        }

        public void Ok(object sender, RoutedEventArgs args) {
            error.IsVisible = false;
            DisableControls();

            Uri.TryCreate(url.Text, UriKind.Absolute, out Uri? uri);
            if (uri == null || uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps) {
                error.Text = "The URL is invalid";
                error.IsVisible = true;
                EnableControls();
                return;
            }

            string filename = Utils.RemoveYamlExt(name.Text);

            if (filename.IndexOfAny(Path.GetInvalidFileNameChars()) > 0) {
                error.Text = "The name contains invalid characters";
                error.IsVisible = true;
                EnableControls();
            }

            if (ConfigFile.GetClashConfigs().Select(Utils.RemoveYamlExt).Contains(filename)) {
                error.Text = "A config with the same name exists";
                error.IsVisible = true;
                EnableControls();
            }

            Task.Run(async delegate {
                try {
                    var data = await Utils.HttpClient.GetByteArrayAsync(uri);

                    string? err = Clash.ValidateConfig(data);
                    if (err != null) {
                        throw new Exception(err);
                    }

                    Dispatcher.UIThread.Post(delegate {
                        Close(new Result($"{filename}.yaml", uri.ToString(), data));
                    });
                } catch (Exception e) {
                    Dispatcher.UIThread.Post(delegate {
                        error.Text = e.Message;
                        error.IsVisible = true;
                        EnableControls();
                    });
                }
            });
        }

        public void Cancel(object sender, RoutedEventArgs args) {
            Close(null);
        }
    }
}

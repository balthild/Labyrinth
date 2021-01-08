using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Threading;
using Labyrinth.Controls;
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
            HideTaskbarIcon();

            InitializeComponent();

            url = this.FindControl<TextBox>("Url");
            name = this.FindControl<TextBox>("Name");
            error = this.FindControl<TextBlock>("Error");
            ok = this.FindControl<Button>("Ok");
            cancel = this.FindControl<Button>("Cancel");
        }

        private void InitializeComponent() {
            AvaloniaXamlLoader.Load(this);

            this.FindControl<Control>("Title").PointerPressed += (_, e) => {
                if ((e.Source as Control)?.Classes.Contains("drag") ?? false) {
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
            url.Foreground = Brushes.Black;

            name.IsEnabled = true;
            name.Foreground = Brushes.Black;

            ok.IsEnabled = true;
            cancel.IsEnabled = true;
        }

        private void ShowError(string message) {
            error.Text = message;
            error.IsVisible = true;
        }

        public void Ok(object sender, RoutedEventArgs args) {
            error.IsVisible = false;
            DisableControls();

            Uri.TryCreate(url.Text, UriKind.Absolute, out Uri? uri);
            if (uri == null || uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps) {
                ShowError("The URL is invalid");
                EnableControls();
                return;
            }

            string filename = Utils.RemoveYamlExt(name.Text ?? "");
            if (filename.Length == 0) {
                ShowError("Please specify a profile name.");
                EnableControls();
                return;
            }

            if (filename.IndexOfAny(Path.GetInvalidFileNameChars()) > 0) {
                ShowError("The name contains invalid characters.");
                EnableControls();
                return;
            }

            if (ConfigFile.GetClashConfigs().Select(Utils.RemoveYamlExt).Contains(filename)) {
                ShowError("The profile name has existed. Please specify another name.");
                EnableControls();
                return;
            }

            Task.Run(async delegate {
                try {
                    byte[] data = await Utils.GetProxiedHttpClient().GetByteArrayAsync(uri);

                    string? err = Clash.ValidateConfig(data);
                    if (err != null) {
                        throw new Exception(err);
                    }

                    Dispatcher.UIThread.Post(() => {
                        Close(new Result($"{filename}.yaml", uri.ToString(), data));
                    });
                } catch (Exception e) {
                    Dispatcher.UIThread.Post(() => {
                        ShowError(e.Message);
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

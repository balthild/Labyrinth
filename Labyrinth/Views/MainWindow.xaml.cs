using System;
using System.Linq;
using System.Runtime.InteropServices;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Xaml.Interactivity;
using Labyrinth.Support.Interop;

namespace Labyrinth.Views {
    public class MainWindow : Window {
        private WinApi.WndProc? subclassWndProc;

        public MainWindow() {
            WindowEffects();
            InitializeComponent();

#if DEBUG
            this.AttachDevTools();
#endif
        }

        private void WindowEffects() {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
                WindowEffectsWin32();
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX)) {
                // TODO
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) {
                // TODO
            }
        }

        private void WindowEffectsWin32() {
            int dwmEnabled = 0;
            WinApi.DwmIsCompositionEnabled(ref dwmEnabled);
            if (dwmEnabled == 0)
                return;

            IntPtr hWnd = PlatformImpl.Handle.Handle;

            // Disable window maximizing
            uint style = WinApi.GetWindowLongPtr(hWnd, WinApi.GWL_STYLE);
            style &= ~WinApi.WS_MAXIMIZEBOX;
            WinApi.SetWindowLongPtr(hWnd, WinApi.GWL_STYLE, style);

            // Prevent the delegate being garbage collected
            subclassWndProc = Win32SubclassWndProc;
            // Make window looks "border-less" but keep effects such as aero peek and minimizing animations
            WinApi.SetWindowSubclass(hWnd, subclassWndProc, IntPtr.Zero, IntPtr.Zero);

            // Allow drawing in non-client area
            int pv = 2;
            WinApi.DwmSetWindowAttribute(hWnd, 2, ref pv, 4);

            // Enable window shadows
            var m = new WinApi.Margins { Left = 1, Right = 1, Top = 1, Bottom = 1 };
            WinApi.DwmExtendFrameIntoClientArea(hWnd, ref m);
        }

        private void InitializeComponent() {
            AvaloniaXamlLoader.Load(this);

            this.FindControl<Control>("Title").PointerPressed += (i, e) => {
                if (((Control) e.Source).Classes.Contains("drag")) {
                    BeginMoveDrag(e);
                }
            };
        }

        public void AddCurrentTabClass(object target, AvaloniaPropertyChangedEventArgs args) {
            var behavior = (Behavior) args.Sender;
            var tab = (Control) behavior.AssociatedObject!;
            tab?.Classes.Add("current");
        }

        public void RemoveCurrentTabClass(object target, AvaloniaPropertyChangedEventArgs args) {
            var behavior = (Behavior) args.Sender;
            var tab = (Control) behavior.AssociatedObject!;
            tab?.Classes.Remove("current");
        }

        private static IntPtr Win32SubclassWndProc(IntPtr hWnd, uint uMsg, IntPtr wParam, IntPtr lParam) {
            return uMsg switch {
                WinApi.WM_NCCALCSIZE => IntPtr.Zero,
                _ => WinApi.DefSubclassProc(hWnd, uMsg, wParam, lParam),
            };
        }
    }
}

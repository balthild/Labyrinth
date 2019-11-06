using System;
using System.Runtime.InteropServices;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Labyrinth.Support;
using Labyrinth.ViewModels;

namespace Labyrinth.Views {
    public class MainWindow : Window {
        private WindowsInterop.WndProc subclassWndProc;

        public MainWindow() {
            DataContext = new MainWindowViewModel();

            WindowEffects();
            InitializeComponent();

#if DEBUG
            this.AttachDevTools();
#endif
        }

        private void WindowEffects() {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
                int dwmEnabled = 0;
                WindowsInterop.DwmIsCompositionEnabled(ref dwmEnabled);
                if (dwmEnabled == 0)
                    return;

                IntPtr hWnd = PlatformImpl.Handle.Handle;

                // Prevent the delegate being garbage collected
                subclassWndProc = SubclassWndProc;
                // Make window looks "border-less" but keep effects such as aero peek and minimizing animations
                WindowsInterop.SetWindowSubclass(hWnd, subclassWndProc, IntPtr.Zero, IntPtr.Zero);

                // Allow drawing in non-client area
                int pv = 2;
                WindowsInterop.DwmSetWindowAttribute(hWnd, 2, ref pv, 4);

                // Enable window shadows
                var m = new WindowsInterop.Margins {Left = 1, Right = 1, Top = 1, Bottom = 1};
                WindowsInterop.DwmExtendFrameIntoClientArea(hWnd, ref m);
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX)) {
                // TODO
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) {
                // TODO
            }
        }

        private IntPtr SubclassWndProc(IntPtr hWnd, uint uMsg, IntPtr wParam, IntPtr lParam) {
            switch ((WindowsInterop.WindowMsg) uMsg) {
                case WindowsInterop.WindowMsg.WM_NCCALCSIZE:
                    return IntPtr.Zero;
                default:
                    return WindowsInterop.DefSubclassProc(hWnd, uMsg, wParam, lParam);
            }
        }

        private void InitializeComponent() {
            AvaloniaXamlLoader.Load(this);

            this.FindControl<Control>("Title").PointerPressed += (i, e) => { BeginMoveDrag(e); };
        }
    }
}
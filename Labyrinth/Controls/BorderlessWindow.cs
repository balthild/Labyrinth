using System;
using System.Globalization;
using System.Runtime.InteropServices;
using Avalonia.Controls;
using static Labyrinth.Support.Interop.WinApi;

namespace Labyrinth.Controls {
    public class BorderlessWindow : Window {
        // Prevent the delegate being garbage collected
        private static readonly WndProc SubclassWndProc = Win32SubclassWndProc;

        internal BorderlessWindow() {
            SetLocalizedFont();
            WindowEffects();
        }

        private void SetLocalizedFont() {
            CultureInfo culture = CultureInfo.InstalledUICulture;
            if (culture.ThreeLetterWindowsLanguageName == "CHS")
                FontFamily = "Segoe UI, Microsoft Yahei UI, PingFang SC, " +
                             "Noto Sans CJK SC, Source Han Sans, Source Han Sans CN, sans-serif";
        }

        private void WindowEffects() {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
                WindowEffectsWin32();
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX)) {
                SystemDecorations = SystemDecorations.BorderOnly;
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) {
                // TODO
            }
        }

        private void WindowEffectsWin32() {
            int dwmEnabled = 0;
            DwmIsCompositionEnabled(ref dwmEnabled);
            if (dwmEnabled == 0)
                return;

            IntPtr hWnd = PlatformImpl.Handle.Handle;

            // Disable window maximizing
            uint style = GetWindowLongPtr(hWnd, WindowLongIndex.GWL_STYLE);
            style &= (uint) ~WindowStyle.WS_MAXIMIZEBOX;
            SetWindowLongPtr(hWnd, WindowLongIndex.GWL_STYLE, style);

            // Make window looks "border-less" but keep effects such as aero peek and minimizing animations
            SetWindowSubclass(hWnd, SubclassWndProc, IntPtr.Zero, IntPtr.Zero);

            // Allow drawing in non-client area
            int pv = 2;
            DwmSetWindowAttribute(hWnd, 2, ref pv, 4);

            // Enable window shadows
            var m = new Margins { Left = 1, Right = 1, Top = 1, Bottom = 1 };
            DwmExtendFrameIntoClientArea(hWnd, ref m);
        }

        protected new void Activate() {
            base.Activate();

            // Window activation does not work properly on Windows.
            // See https://github.com/AvaloniaUI/Avalonia/issues/2975
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
                IntPtr hWnd = PlatformImpl.Handle.Handle;
                SetForegroundWindow(hWnd);
            }
        }

        private static IntPtr Win32SubclassWndProc(IntPtr hWnd, WindowMessage uMsg, IntPtr wParam, IntPtr lParam) {
            return uMsg switch {
                WindowMessage.WM_NCCALCSIZE => IntPtr.Zero,
                _ => DefSubclassProc(hWnd, uMsg, wParam, lParam),
            };
        }
    }
}
